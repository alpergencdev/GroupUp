﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.Models.LocationModels;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
            var targetGroup = _context.Groups.Include(g => g.Members).Include(g => g.Members.Select( u => u.AspNetIdentity) ).SingleOrDefault(g => g.GroupId == id);
            GroupDetailsViewModel gvm = new GroupDetailsViewModel()
            {
                Group = targetGroup,
                User = currentUser
            };
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
            var gvm = new GroupDetailsViewModel()
            {
                Group = new Group()
                {
                    City = locationProperties.City,
                    Country = locationProperties.CountryLongName,
                    Continent = locationProperties.Continent
                }
            };
            return View(gvm);
        }

        [HttpPost]
        public ActionResult Save(GroupDetailsViewModel gvm)
        {
            string aspUserId = User.Identity.GetUserId();
            var user = _context.Users.Include(u => u.AspNetIdentity).SingleOrDefault(u => u.AspNetIdentity.Id == aspUserId);
            gvm.Group.Creator = user;
            gvm.Group.Members = new List<User>();
            gvm.Group.Members.Add(user);
            if (!ModelState.IsValid)
            {
                bool allInputsAreValid = true;
                for(int i = 0; i < ModelState.Values.Count - 2; i++)
                {
                    if (ModelState.Values.ElementAt(i).Errors.Count > 0)
                    {
                        allInputsAreValid = false;
                        break;
                    }
                }

                if (allInputsAreValid)
                {
                    try
                    {
                        Group group = new Group()
                        {
                            Title = gvm.Group.Title,
                            Description = gvm.Group.Description,
                            MaxUserCapacity = gvm.Group.MaxUserCapacity,
                            City = gvm.Group.City,
                            Continent = gvm.Group.Continent,
                            Country = gvm.Group.Country,
                            Members = new List<User>()
                            {
                                user
                            },
                            Creator = user
                        };
                        _context.Groups.Add(group);
                        _context.SaveChanges();
                        return View("Index");
                    }
                    catch (ValidationException)
                    {
                        return View("Create", gvm);
                    }
                }
            }

            _context.Groups.Add(gvm.Group);
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
                .Where(g => g.Members.Count < g.MaxUserCapacity).ToList(); // && CORRESPONDING TO USER'S LOCATION
            var viewModel = new GroupRequestsViewModel()
            {
                Groups = groupsToShow,
                User = currentUser,
                Location = (LocationProperties) Session["Location"]
            };
            return View(viewModel);
        }

        [Authorize]
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
                .Where(g => g.Creator.UserId == currentUser.UserId).ToList();

            var joinedGroups = _context.Groups.Include(g => g.Members).Include(g => g.Creator).ToList();
            joinedGroups.RemoveAll(g => currentUser != null && (!g.Members.Contains(currentUser) || g.Creator.UserId == currentUser.UserId));

            var viewModel = new UserGroupsViewModel()
            {
                CreatedGroups = createdGroups.ToList(),
                JoinedGroups = joinedGroups.ToList()
            };

            return View(viewModel);
        }

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

        public ActionResult PrintLocation()
        {
            var location = (LocationProperties) Session["Location"];

            if (location == null)
            {
                return HttpNotFound();
            }

            return Content($"Lat: {location.Lat} Lng: {location.Lng} City: {location.City} Country = {location.CountryLongName} Continent = {location.Continent}");
        }
    }

   
}