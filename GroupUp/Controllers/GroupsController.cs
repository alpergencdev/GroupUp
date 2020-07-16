using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.Models.LocationModels;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
// ReSharper disable SimplifyLinqExpression

namespace GroupUp.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        public GroupsController()
        {
            _context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this._context));
        }
        // GET: Groups
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            var targetGroup = _context.Groups.Include(g => g.Creator).Include(g => g.Members).Include(g => g.Members.Select( u => u.AspNetIdentity) ).SingleOrDefault(g => g.GroupId == id);
            if (targetGroup != null && targetGroup.IsClosed)
            {
                return RedirectToAction("ClosedDetails", new {groupId = targetGroup.GroupId});
            }
            GroupDetailsViewModel gvm = new GroupDetailsViewModel()
            {
                Group = targetGroup,
                User = currentUser
            };
            if (targetGroup != null && targetGroup.Creator == currentUser)
            {
                return View("CreatorDetails", gvm);
            }
            if (targetGroup != null && targetGroup.Members.Contains(currentUser))
            {
                return View("MemberDetails", gvm);
            }

            
            return View(gvm);
        }

        [Authorize]
        public ActionResult Create()
        {
            if (Session["Location"] == null)
            {
                Session["GetLocationFlag"] = true;
                return RedirectToAction("GetLocation", new { returnAction = "Create" });
            }

            var locationProperties = (LocationProperties) Session["Location"];
            var viewModel = new CreateGroupViewModel()
            {
                GroupId = -1,
                City = locationProperties.City,
                Country = locationProperties.CountryLongName,
                Continent = locationProperties.Continent,
                Title = "",
                DetailedDescription = "",
                MaxUserCapacity = 0
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Save(CreateGroupViewModel viewModel)
        {
            string aspUserId = User.Identity.GetUserId();
            var user = _context.Users.Include(u => u.AspNetIdentity).SingleOrDefault(u => u.AspNetIdentity.Id == aspUserId);
            if (!ModelState.IsValid)
            {
                if (viewModel.GroupId < 0)
                {
                    return View("Create", viewModel);
                }
                else
                {
                    return HttpNotFound();
                }
            }

            if (viewModel.GroupId < 0)
            {
                Group newGroup = new Group()
                {
                    Title = viewModel.Title,
                    Description = viewModel.DetailedDescription,
                    MaxUserCapacity = viewModel.MaxUserCapacity,
                    City = viewModel.City,
                    Continent = viewModel.Continent,
                    Country = viewModel.Country,
                    Members = new List<User>()
                    {
                        user
                    },
                    Creator = user
                };
                _context.Groups.Add(newGroup);
            }
            else
            {
                var groupInDb = _context.Groups.SingleOrDefault(g => g.GroupId == viewModel.GroupId);
                if (groupInDb == null)
                {
                    return HttpNotFound();
                }

                groupInDb.Title = viewModel.Title;
                groupInDb.Description = viewModel.DetailedDescription;
            }
            _context.SaveChanges();
            return View("Index");
        }

        [Authorize]
        public ActionResult Requests()
        {
            if (Session["Location"] == null)
            {
                Session["GetLocationFlag"] = true;
                return RedirectToAction("GetLocation", new {returnAction = "Requests"});
            }

            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            var groupsToShow = _context.Groups.Include(g => g.Members)
                .Where(g => g.Members.Count < g.MaxUserCapacity && !g.IsClosed).ToList();
            var viewModel = new GroupRequestsViewModel()
            {
                Groups = groupsToShow, // location and membership checks are done inside razorpages.
                User = currentUser,
                Location = (LocationProperties) Session["Location"]
            };
            return View(viewModel);
        }

        [Authorize(Roles="SecurityLevel1")]
        [HttpPost]
        public ActionResult Join(int groupId)
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            var targetGroup = _context.Groups.Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);

            if (targetGroup == null)
            {
                return Content("There is no group with the given ID.");
            }

            if (targetGroup.IsClosed)
            {
                return Content("This group is closed.");
            }
            if (currentUser == null)
            {
                return Content("Please log in.");
            }

            if (targetGroup.Members.Contains(currentUser))
            {
                return Content("User is already a part of this group.");
            }

            if (targetGroup.Members.Count >= targetGroup.MaxUserCapacity)
            {
                return Content("Group is full.");
            }

            // If no erroneous conditions were satisfied...

            targetGroup.Members.Add(currentUser);
            currentUser.Groups.Add(targetGroup);
            _context.SaveChanges();
            return RedirectToAction("Details", "Groups", new { id = groupId});
        }

        [Authorize]
        public bool CanUserJoin(int groupId)
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            var targetGroup = _context.Groups.Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);

            if (targetGroup == null)
            {
                return false;
            }

            if (currentUser == null)
            {
                return false;
            }

            if (targetGroup.Members.Contains(currentUser))
            {
                return false;
            }

            return true;
        }

        [Authorize]
        public ActionResult UserGroups()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups)
                .SingleOrDefault( u => u.AspNetIdentity.Id == aspNetId);

            var createdGroups = _context.Groups.Include(g => g.Members)
                .Where(g => g.Creator.UserId == currentUser.UserId && !g.IsClosed).ToList();

            var joinedGroups = _context.Groups.Include(g => g.Members).Include(g => g.Creator).ToList();
            joinedGroups.RemoveAll(g => currentUser != null && (!g.Members.Contains(currentUser) || g.Creator.UserId == currentUser.UserId || g.IsClosed));

            var closedGroups = _context.Groups.Include(g => g.Members).Where(g => g.IsClosed).ToList();

            var closedUserGroups = new List<Group>();
            foreach (var group in closedGroups)
            {
                if (group.Members.Contains(currentUser))
                {
                    var ratedUsers = _context.ClosedGroups.Include(cg => cg.RatedUsers)
                        .SingleOrDefault(cg => cg.Group.GroupId == group.GroupId)
                        ?.RatedUsers;
                    if (ratedUsers != null && !ratedUsers.Contains(currentUser))
                    {
                        closedUserGroups.Add(group);
                    }
                }
            }
            var viewModel = new UserGroupsViewModel()
            {
                User = currentUser,
                CreatedGroups = createdGroups.ToList(),
                JoinedGroups = joinedGroups.ToList(),
                ClosedGroups = closedUserGroups
                
            };

            return View(viewModel);
        }

        [Authorize]
        public ActionResult GetLocation(string returnAction)
        {
            if (Session["GetLocationFlag"] != null && (bool)Session["GetLocationFlag"])
            {
                var viewModel = new GetLocationViewModel()
                {
                    ReturnAction = returnAction
                };
                Session["GetLocationFlag"] = false;
                return View(viewModel);
            }

            return HttpNotFound();
        }

        [Authorize]
        public ActionResult ReadLocation(double lat, double lng, string returnToAction)
        {
            var locationProperties = new LocationProperties()
            {
                Lat = lat,
                Lng = lng,
                City = "-",
                CountryLongName = "-",
                CountryShortName = "-",
                Continent = "-"
            };

            LocationMethods.ReverseGeocode(lat, lng, ref locationProperties);
            Session["Location"] = locationProperties;
            return Content(lat + " " + lng);
            }

        [Authorize]
        public ActionResult PrintLocation()
        {
            var location = (LocationProperties) Session["Location"];

            if (location == null)
            {
                return HttpNotFound();
            }

            return Content($"Lat: {location.Lat} Lng: {location.Lng} City: {location.City} Country = {location.CountryLongName} Continent = {location.Continent}");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Close(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }
            var targetGroup = _context.Groups.Include(g => g.Creator).SingleOrDefault(g => g.GroupId == groupId);

            if (targetGroup == null)
            {
                return HttpNotFound();
            }

            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            if (targetGroup.Creator != currentUser)
            {
                return HttpNotFound();
            }

            targetGroup.IsClosed = true;
            ClosedGroup newEntry = new ClosedGroup()
            {
                Group = targetGroup,
                RatedUsers = new List<User>()
            };
            _context.ClosedGroups.Add(newEntry);
            _context.SaveChanges();
            return RedirectToAction("UserGroups", "Groups");
        }

        [Authorize]
        public ActionResult ClosedDetails(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var closedGroupEntry = _context.ClosedGroups
                .Include(cg => cg.RatedUsers)
                .Include(cg => cg.Group)
                .Include(cg => cg.Group.Members)
                .Include(cg => cg.Group.Members.Select(u => u.AspNetIdentity))
                .SingleOrDefault(cg => cg.Group.GroupId == groupId);

            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            if (closedGroupEntry == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ClosedGroupViewModel()
            {
                ClosedGroup = closedGroupEntry,
                UserAlreadyRated = closedGroupEntry.RatedUsers.Contains(currentUser),
                User = currentUser
            };

            return View(viewModel);
        }

        [Authorize]
        public ActionResult Rate(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .Include( g=> g.Groups)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            if (currentUser == null)
            {
                return HttpNotFound();
            }

            if (!currentUser.Groups.Any(g => g.GroupId == groupId))
            {
                return HttpNotFound();
            }
            else
            {
                var targetGroup = _context.Groups.Include(g => g.Members)
                    .Include(g => g.Members.Select(u => u.AspNetIdentity))
                    .SingleOrDefault(g => g.GroupId == groupId);
                if (targetGroup == null)
                {
                    return HttpNotFound();
                }

                if (!targetGroup.IsClosed)
                {
                    return HttpNotFound();
                }
                else
                {
                    var viewModel = new GroupRatingViewModel()
                    {
                        UserRatings = new Dictionary<string, int>(),
                        GroupId = targetGroup.GroupId
                    };
                    foreach (var member in targetGroup.Members)
                    {
                        if (member.UserId != currentUser.UserId)
                        {
                            viewModel.UserRatings.Add(member.AspNetIdentity.UserName, 0);
                        }
                    }
                    return View(viewModel);
                }
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult PostRating(GroupRatingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Rate", viewModel);
            }
            else
            {
                foreach (var kvp in viewModel.UserRatings)
                {
                    var user = _context.Users.SingleOrDefault(u => u.AspNetIdentity.UserName == kvp.Key);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    user.TrustPoints += (kvp.Value - 5) * 5;
                }

                var closedGroup = _context.ClosedGroups
                    .Include(cg => cg.RatedUsers)
                    .SingleOrDefault(cg => cg.Group.GroupId == viewModel.GroupId);

                var aspNetId = User.Identity.GetUserId();
                var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
                if (currentUser == null || closedGroup == null)
                {
                    return HttpNotFound();
                }
                closedGroup.RatedUsers.Add(currentUser);
                _context.SaveChanges();
                return RedirectToAction("UserGroups");
            }
        }

    }

   
}