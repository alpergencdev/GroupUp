using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;

namespace GroupUp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController()
        {
            _context = new ApplicationDbContext();
        }
        
        [Authorize]
        public ActionResult Details(int userId)
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups).SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            var targetUser = _context.Users.Include(u => u.Groups).Include(u => u.AspNetIdentity).SingleOrDefault(u => u.UserId == userId);
            UserDetailsViewModel viewModel = null;

            if (targetUser == null)
            {
                viewModel = new UserDetailsViewModel()
                {
                    DenyMessage = "User with the given ID does not exist.",
                    UserCanView = false,
                    User = null
                };
            }
            else if (currentUser != null && !currentUser.Groups.Intersect(targetUser.Groups).Any())
            {
                viewModel = new UserDetailsViewModel()
                {
                    DenyMessage = "You do not share a group with this user.",
                    UserCanView = false,
                    User = null
                };
            }
            else
            {
                viewModel = new UserDetailsViewModel()
                {
                    DenyMessage = "",
                    UserCanView = true,
                    User = targetUser
                };
            }

            return View(viewModel);
        }
    }
}