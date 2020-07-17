using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.ViewModels;

namespace GroupUp.Controllers
{
    [Authorize(Roles="Moderator")]
    public class ModeratorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModeratorsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Moderators
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            
            var users = _context.Users.Include(u => u.AspNetIdentity).ToList();
            var viewModel = new ModeratorUsersViewModel()
            {
                Users = users
            };

            return View(viewModel);
        }

        public ActionResult UserDetails(int? userId)
        {
            if (!userId.HasValue)
            {
                return HttpNotFound();
            }

            var targetUser = _context.Users
                .Include(u => u.AspNetIdentity)
                .Include(u => u.Groups)
                .Include(u => u.Groups.Select(g => g.Creator))
                .SingleOrDefault(u => u.UserId == userId);

            if (targetUser == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ModeratorUserDetailsViewModel()
            {
                User = targetUser
            };

            return View(viewModel);
        }

        public ActionResult ChangeLockoutDate(int? userId)
        {
            if (!userId.HasValue)
            {
                return HttpNotFound();
            }

            var targetUser = _context.Users
                .Include(u => u.AspNetIdentity)
                .SingleOrDefault(u => u.UserId == userId);

            if (targetUser == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ModeratorLockoutViewModel()
            {
                UserId = (int) userId
            };

            if (targetUser.AspNetIdentity.LockoutEndDateUtc != null)
            {
                viewModel.LockoutDate = (DateTime) targetUser.AspNetIdentity.LockoutEndDateUtc;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult PostLockoutDate(ModeratorLockoutViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ChangeLockoutDate", viewModel);
            }

            else
            {
                var targetUser = _context.Users.Include(u => u.AspNetIdentity)
                    .SingleOrDefault(u => u.UserId == viewModel.UserId);
                if (targetUser == null)
                {
                    return HttpNotFound();
                }

                // If the target user was not locked out before, but is so now,
                // cut their trust points in half as an extra punishment.
                if ((targetUser.AspNetIdentity.LockoutEndDateUtc == null ||
                    targetUser.AspNetIdentity.LockoutEndDateUtc < DateTime.Now)
                    && viewModel.LockoutDate > DateTime.Now)
                {
                    targetUser.TrustPoints /= 2;
                }

                targetUser.AspNetIdentity.LockoutEndDateUtc = viewModel.LockoutDate;

                _context.SaveChanges();
                return RedirectToAction("UserDetails", new {userId = viewModel.UserId});
            }
        }

        public ActionResult Groups()
        {
            var users = _context.Users.Include(u => u.AspNetIdentity).ToList();
            var viewModel = new ModeratorUsersViewModel()
            {
                Users = users
            };

            return View(viewModel);
        }
    }
}