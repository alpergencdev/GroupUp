﻿using System;
using System.Data.Entity;
using System.Linq;
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
        
        [Authorize(Roles="SecurityLevel2")]
        public ActionResult Details(int userId)
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.Groups).SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            var targetUser = _context.Users.Include(u => u.Groups).Include(u => u.AspNetIdentity).SingleOrDefault(u => u.UserId == userId);
            UserDetailsViewModel viewModel;

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

        [Authorize]
        public ActionResult VerifyUser()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            if (currentUser != null && currentUser.IsVerified)
            {
                return RedirectToAction("UserGroups", "Groups");
            }
            var viewModel = new VerifyUserViewModel();
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult PostVerify(VerifyUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyUser", viewModel);
            }
            else
            {
                var aspNetId = User.Identity.GetUserId();
                var currentUser = _context.Users.SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
                if (currentUser != null && (!currentUser.IsVerified && currentUser.VerificationCode == viewModel.VerificationCode))
                {
                    // verify the user
                    currentUser.IsVerified = true;
                    currentUser.VerificationCode = null;
                    _context.SaveChanges();
                    return RedirectToAction("UserGroups", "Groups");
                }

                return RedirectToAction("UserGroups", "Groups");

            }
        }

        [Authorize]
        public ActionResult IncreaseSecurityLevel()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            var viewModel = new ISLViewModel()
            {
                User = currentUser
            };
            return View(viewModel);
        }

        [Authorize]
        public ActionResult IncreaseSecurityLevelTo1()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            if (currentUser == null)
            {
                return HttpNotFound();
            }
            if (currentUser.SecurityLevel > 1)
            {
                return Content("Your security level is higher than 1.");
            }

            if (!currentUser.IsVerified)
            {
                return Content("You do not meet the requirements to increase your security level.");
            }
            // THEN
            currentUser.SecurityLevel = 1;
            _context.SaveChanges();
            return RedirectToAction("IncreaseSecurityLevelTo1", "Account");
        }

        [Authorize]
        public ActionResult IncreaseSecurityLevelTo2()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            if (currentUser == null)
            {
                return HttpNotFound();
            }
            if (currentUser.SecurityLevel > 2 )
            {
                return Content("Your security level is higher than 2.");
            }

            if (currentUser.TrustPoints < 50)
            {
                return Content("You do not meet the requirements to increase your security level.");
            }

            // THEN
            currentUser.SecurityLevel = 2;
            _context.SaveChanges();
            return RedirectToAction("IncreaseSecurityLevelTo2", "Account");
        }

        [Authorize]
        public ActionResult ResendVerificationEmail()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity).SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);
            if (currentUser != null && currentUser.VerificationCode != null && !currentUser.IsVerified)
            {
                EmailSender.Send(currentUser.AspNetIdentity.Email, (int) currentUser.VerificationCode);
            }

            return RedirectToAction("VerifyUser");
        }

        [Authorize]
        public ActionResult Edit()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            if (currentUser == null)
            {
                return HttpNotFound();
            }

            var viewModel = new UserEditViewModel()
            {
                ContactInfo = currentUser.ContactInfo,
                Email = currentUser.AspNetIdentity.Email
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult PostEdit(UserEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", viewModel);
            }

            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users.Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            if (currentUser == null)
            {
                return HttpNotFound();
            }

            currentUser.ContactInfo = viewModel.ContactInfo;
            if (currentUser.AspNetIdentity.Email != viewModel.Email && !currentUser.IsVerified)
            {
                if (currentUser.VerificationCode != null)
                    EmailSender.Send(viewModel.Email, (int) currentUser.VerificationCode);
            }
            currentUser.AspNetIdentity.Email = viewModel.Email;
            _context.SaveChanges();
            return RedirectToAction("Index", "Manage");
        }
    }
}