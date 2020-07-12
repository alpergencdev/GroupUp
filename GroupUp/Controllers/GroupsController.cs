using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GroupUp.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext _context;
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

        public ActionResult Details(int id)
        {
            GroupViewModel gvm = new GroupViewModel()
            {
                Group = _context.Groups.Include(g => g.Members).SingleOrDefault()
            };
            return View(gvm);
        }
        [Authorize]
        public ActionResult Create()
        {
            
            var gvm = new GroupViewModel()
            {
                Group = new Group(),
            };
            return View(gvm);
        }

        [HttpPost]
        public ActionResult Save(GroupViewModel gvm)
        {
            string aspUserId = User.Identity.GetUserId();
            var user = _context.Users.Include(u => u.AspNetIdentity).SingleOrDefault(u => u.AspNetIdentity.Id == aspUserId);
            gvm.Group.Creator = user;
            gvm.Group.Members = new List<User>();
            gvm.Group.Members.Add(user);
            if (!ModelState.IsValid)
            {
                bool AllInputsAreValid = true;
                for(int i = 0; i < ModelState.Values.Count - 2; i++)
                {
                    if (ModelState.Values.ElementAt(i).Errors.Count > 0)
                    {
                        AllInputsAreValid = false;
                        break;
                    }
                }

                if (AllInputsAreValid)
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
    }
}