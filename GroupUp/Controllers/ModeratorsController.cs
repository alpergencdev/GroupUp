using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;


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

            // get the user with the given ID with all the necessary navigation properties.
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
                /* Lockout dates work as follows: A lockout date value that is NULL,
                 * or is before the current time means that the user can access the system.
                 * A lockout date that is in the future means the user cannot access the system
                 * until that date is reached. The moderator can use this to block/unblock a user.
                */
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
            var groups = _context.Groups.ToList();
            var viewModel = new ModeratorGroupsViewModel()
            {
                Groups = groups
            };

            return View(viewModel);
        }

        public ActionResult GroupDetails(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            // get the group with the given ID with all the necessary navigation properties.
            var targetGroup = _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Members.Select(u => u.AspNetIdentity))
                .Include(g => g.Creator)
                .SingleOrDefault(g => g.GroupId == groupId);

            if (targetGroup == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ModeratorGroupDetailsViewModel()
            {
                Group = targetGroup
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteGroup(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var targetGroup = _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Members.Select(u => u.AspNetIdentity))
                .Include(g => g.Creator)
                .SingleOrDefault(g => g.GroupId == groupId);

            if (targetGroup == null)
            {
                return HttpNotFound();
            }

            // if there are any closed group objects targeting this group object, delete them.
            if (_context.ClosedGroups.Any(cg => cg.Group.GroupId == targetGroup.GroupId))
            {
                _context.ClosedGroups.RemoveRange(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _context.ClosedGroups.Where(cg => cg.Group.GroupId == targetGroup.GroupId));
            }

            // if there are any group report objects targeting this group object, delete them.
            if (_context.GroupReports.Any(gr => gr.TargetGroup.GroupId == targetGroup.GroupId))
            {
                _context.GroupReports.RemoveRange(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _context.GroupReports.Where(gr => gr.TargetGroup.GroupId == targetGroup.GroupId));
            }

            // keep removing first item until there are no more members.
            while (targetGroup.Members.Count > 0)
            {
                targetGroup.Members.RemoveAt(0);
            }

            // finally, delete group from database.
            _context.Groups.Remove(targetGroup);
            _context.SaveChanges();
            return RedirectToAction("Groups");
        }

        public ActionResult UserReports()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUserId = _context.Users
                .Where(u => u.AspNetIdentity.Id == aspNetId)
                .Select(u => u.UserId)
                .SingleOrDefault();

            var userReports = _context.UserReports
                .Include(ur => ur.TargetUser)
                .Include(ur => ur.TargetUser.AspNetIdentity)
                .Where(ur => ur.TargetUser.UserId != currentUserId).ToList();

            var viewModel = new ModeratorUserReportsViewModel()
            {
                UserReports = userReports
            };

            return View(viewModel);
        }

        public ActionResult UserReportDetails(int? userReportId)
        {
            if (!userReportId.HasValue)
            {
                return HttpNotFound();
            }

            var targetReport = _context.UserReports
                .Include(ur => ur.TargetUser)
                .Include(ur => ur.TargetUser.AspNetIdentity)
                .SingleOrDefault(ur => ur.UserReportId == userReportId);

            if (targetReport == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ModeratorURDetailsViewModel()
            {
                UserReport = targetReport
            };

            return View(viewModel);
        }

        public ActionResult DeleteUserReport(int? userReportId)
        {
            if (!userReportId.HasValue)
            {
                return HttpNotFound();
            }

            var targetReport = _context.UserReports
                .Include(ur => ur.TargetUser)
                .Include(ur => ur.TargetUser.AspNetIdentity)
                .SingleOrDefault(ur => ur.UserReportId == userReportId);

            if (targetReport == null)
            {
                return HttpNotFound();
            }

            _context.UserReports.Remove(targetReport);
            _context.SaveChanges();
            return RedirectToAction("UserReports");
        }

        public ActionResult GroupReports()
        {
            var aspNetId = User.Identity.GetUserId();
            var currentUser = _context.Users
                .SingleOrDefault(u => u.AspNetIdentity.Id == aspNetId);

            var groupReports = _context.GroupReports
                .Include(gr => gr.TargetGroup)
                .Include(gr => gr.TargetGroup.Members)
                .ToList();

            groupReports.RemoveAll(gr => gr.TargetGroup.Members.Contains(currentUser));
            var viewModel = new ModeratorGroupReportsViewModel()
            {
                GroupReports = groupReports
            };

            return View(viewModel);
        }

        public ActionResult GroupReportDetails(int? groupReportId)
        {
            if (!groupReportId.HasValue)
            {
                return HttpNotFound();
            }

            var targetReport = _context.GroupReports
                .Include(gr => gr.TargetGroup)
                .SingleOrDefault(gr => gr.GroupReportId == groupReportId);

            if (targetReport == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ModeratorGRDetailsViewModel()
            {
                GroupReport = targetReport
            };

            return View(viewModel);
        }

        public ActionResult DeleteGroupReport(int? groupReportId)
        {
            if (!groupReportId.HasValue)
            {
                return HttpNotFound();
            }

            var targetReport = _context.GroupReports
                .Include(gr => gr.TargetGroup)
                .SingleOrDefault(gr => gr.GroupReportId == groupReportId);

            if (targetReport == null)
            {
                return HttpNotFound();
            }

            _context.GroupReports.Remove(targetReport);
            _context.SaveChanges();
            return RedirectToAction("GroupReports");
        }
    }
}