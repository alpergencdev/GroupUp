using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.Models.LocationModels;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
// ReSharper disable SimplifyLinqExpression
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GroupUp.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GroupsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Groups
        [Authorize]
        public ActionResult Details(int id)
        {
            
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            // get target group
            var targetGroup = _context.Groups.Include(g => g.Creator).Include(g => g.Members).Include(g => g.Members.Select( u => u.AspNetIdentity) ).SingleOrDefault(g => g.GroupId == id);
            // get which screen to show. If the group is closed, show closeddetails.
            if (targetGroup != null && targetGroup.IsClosed)
            {
                return RedirectToAction("ClosedDetails", new {groupId = targetGroup.GroupId});
            }
            GroupDetailsViewModel gvm = new GroupDetailsViewModel()
            {
                Group = targetGroup,
                User = currentUser
            };
            // if current user is the creator of the desired group, show creator details.
            if (targetGroup != null && targetGroup.Creator == currentUser)
            {
                return View("CreatorDetails", gvm);
            }
            // if current user is a member of the desired group, show member details.
            if (targetGroup != null && targetGroup.Members.Contains(currentUser))
            {
                return View("MemberDetails", gvm);
            }

            // else, show normal details.
            return View(gvm);
        }

        [Authorize]
        public ActionResult Create()
        {
            // if user's location is unknown, get it using the getlocation action.
            if (Session["Location"] == null)
            {
                Session["GetLocationFlag"] = true;
                return RedirectToAction("GetLocation", new { returnAction = "Create" });
            }

            // get the locationproperties object of this session, and create viewmodel using it.
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
            // return back to the screen that posted this request, depending on whether this was an edit or create request.
            if (!ModelState.IsValid)
            {
                if (viewModel.GroupId < 0)
                {
                    return View("Create", viewModel);
                }
                else
                {
                    return View("Edit", viewModel);
                }
            }

            // if groupID < 0, meaning this was a new group, create a new group and put it into the database.
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
            // else, edit the group entry in the database accordingly.
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
            return RedirectToAction("UserGroups", "Groups");
        }

        [Authorize]
        public ActionResult Requests()
        {
            // if user's location is unknown, get it using the getlocation action.
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
                Groups = groupsToShow, // location and membership checks are done inside the view page.
                User = currentUser,
                Location = (LocationProperties) Session["Location"]
            };
            return View(viewModel);
        }

        [Authorize(Roles="SecurityLevel1")]
        public ActionResult Join(int groupId)
        {
            // User must be at least security level 1 to join.
            if (!User.IsInRole("SecurityLevel1"))
            {
                return HttpNotFound();
            }
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            var targetGroup = _context.Groups.Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);

            // check the conditions that would not allow a user to join.
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

            // If no erroneous conditions were satisfied, add user to group and save changes.

            targetGroup.Members.Add(currentUser);
            currentUser.Groups.Add(targetGroup);
            _context.SaveChanges();
            return RedirectToAction("Details", "Groups", new { id = groupId});
        }

        [Authorize]
        public ActionResult UserGroups()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups)
                .SingleOrDefault( u => u.AspNetIdentity.Id == aspNetId);

            // get the created, joined and closed groups of the user seperately.
            var createdGroups = _context.Groups.Include(g => g.Members)
                .Where(g => g.Creator.UserId == currentUser.UserId && !g.IsClosed).ToList();

            var joinedGroups = _context.Groups.Include(g => g.Members).Include(g => g.Creator).ToList();
            joinedGroups.RemoveAll(g => currentUser != null && (!g.Members.Contains(currentUser) || g.Creator.UserId == currentUser.UserId || g.IsClosed));

            var closedGroups = _context.Groups.Include(g => g.Members).Where(g => g.IsClosed).ToList();

            // seperate the closed groups to only get the ones the user has joined and not rated.
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

            // put the lists into a viewmodel and return the view.
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
            // A GetLocation flag is used so that the users can not call this method.
            // A calling action will set this GetLocationFlag to true.
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
        [HttpPost]
        public ActionResult ReadLocation(double lat, double lng)
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
            // My JS code submits (-1,-1) with this error, so we check for errors below:
            if( lat == -1 && lng == -1)
            {
                return RedirectToAction("ErroneousGeolocation", "Account");
            }
            LocationMethods.ReverseGeocode(lat, lng, ref locationProperties);
            Session["Location"] = locationProperties;
            return Content(lat + " " + lng); // does not matter since ReadAction is called by POST inside a JS code.
        }

        [Authorize]
        [HttpPost]
        public ActionResult Close(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }
            var targetGroup = _context.Groups.Include(g => g.Creator).Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);

            if (targetGroup == null)
            {
                return HttpNotFound();
            }

            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            // a user can only close the group is they are the creator of that group.
            if (targetGroup.Creator != currentUser)
            {
                return HttpNotFound();
            }

            // if the code has arrived to this if block, it means that the current user is the creator.
            // here, we check if the creator is the only member inside the group. if so, we delete the group
            // instead of closing it.
            if (targetGroup.Members.Contains(currentUser) && targetGroup.Members.Count == 1)
            {
                targetGroup.Members.Remove(currentUser);
                _context.Groups.Remove(targetGroup);
                _context.SaveChanges();
                return RedirectToAction("UserGroups", "Groups");
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

            // if the user does not have a group with the given ID, return HttpNotFound.
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

        [Authorize(Roles="SecurityLevel1")]
        public ActionResult Leave(int? groupId)
        {
            if (!User.IsInRole("SecurityLevel1"))
            {
                return HttpNotFound();
            }
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var targetGroup = _context.Groups.Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            targetGroup?.Members.Remove(currentUser);
            _context.SaveChanges();
            return RedirectToAction("UserGroups", "Groups");
        }

        [Authorize(Roles="SecurityLevel2")]
        public ActionResult Edit(int? groupId)
        {
            
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var targetGroup = _context.Groups.Include(g => g.Creator)
                .Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            if (targetGroup != null && (currentUser != null && targetGroup.Creator.UserId == currentUser.UserId))
            {
                var viewModel = new CreateGroupViewModel()
                {
                    City = targetGroup.City,
                    Country = targetGroup.Country,
                    Continent = targetGroup.Continent,
                    DetailedDescription = targetGroup.Description,
                    Title = targetGroup.Title,
                    GroupId = targetGroup.GroupId,
                    MaxUserCapacity = targetGroup.MaxUserCapacity
                };
                return View(viewModel);
            }

            return HttpNotFound();
        }

        [Authorize]
        public ActionResult Kick(int? userId, int? groupId)
        {
            if (!userId.HasValue || !groupId.HasValue)
            {
                return HttpNotFound();
            }

            var targetGroup = _context.Groups.Include(g => g.Creator)
                .Include(g => g.Members).SingleOrDefault(g => g.GroupId == groupId);
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            // user can only kick someone out of the group if they created that group.
            if (targetGroup != null && (currentUser != null && targetGroup.Creator.UserId == currentUser.UserId))
            {
                var targetUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
                if (targetGroup.Members.Remove(targetUser))
                {
                    _context.SaveChanges();
                    // ReSharper disable once RedundantAnonymousTypePropertyName
                    return RedirectToAction("Details", new {id = groupId});
                }
                return HttpNotFound();
            }

            return HttpNotFound();
        }
    }

   
}