using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;

namespace GroupUp.Controllers
{
    public class ReportsController : Controller
    {

        public ApplicationDbContext Context
        {
            get => _context;
            set => _context = value;
        }
        private ApplicationDbContext _context;

        public ReportsController()
        {
            _context = new ApplicationDbContext();
        }
        [Authorize]
        public ActionResult ReportUser(int? userId)
        {
            if (!userId.HasValue)
            {
                return HttpNotFound();
            }
            var targetUser = _context.Users.Include(u => u.AspNetIdentity).SingleOrDefault(u => u.UserId == userId);
            if (targetUser == null)
            {
                return HttpNotFound();
            }
            if (targetUser.AspNetIdentity.Id == User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            var viewModel = new UserReportViewModel()
            {
                Reason = "",
                Description = "",
                ReportedUserId = targetUser.UserId,
                TargetUsername = targetUser.AspNetIdentity.UserName
            };
            return View(viewModel);
        }

        [Authorize]
        public ActionResult ReportGroup(int? groupId)
        {
            if (!groupId.HasValue)
            {
                return HttpNotFound();
            }

            var targetGroup = _context.Groups.SingleOrDefault(g => g.GroupId == groupId);
            if (targetGroup != null)
            {
                var viewModel = new GroupReportViewModel()
                {
                    Reason = "",
                    Description = "",
                    ReportedGroupId = targetGroup.GroupId,
                    TargetGroupTitle = targetGroup.Title
                };
                return View(viewModel);
            }

            return HttpNotFound();
        }

        [Authorize(Roles="SecurityLevel1")]
        [HttpPost]
        public ActionResult PostUserReport(UserReportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ReportUser", viewModel);
            }
            else
            {
                var targetUser = _context.Users.SingleOrDefault(u => u.UserId == viewModel.ReportedUserId);
                if (targetUser == null)
                {
                    return View("ReportUser", viewModel);
                }
                UserReport report = new UserReport()
                {
                    DetailedDescription = viewModel.Description,
                    Reason = viewModel.Reason,
                    TargetUser = targetUser
                };
                _context.UserReports.Add(report);
                _context.SaveChanges();
                return RedirectToAction("UserGroups", "Groups");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult PostGroupReport(GroupReportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ReportGroup", viewModel);
            }
            else
            {
                var targetGroup = _context.Groups.SingleOrDefault(g => g.GroupId == viewModel.ReportedGroupId);
                if (targetGroup == null)
                {
                    return View("ReportGroup", viewModel);
                }
                var report = new GroupReport()
                {
                    
                    DetailedDescription = viewModel.Description,
                    Reason = viewModel.Reason,
                    TargetGroup = targetGroup
                };
                _context.GroupReports.Add(report);
                _context.SaveChanges();
                return RedirectToAction("UserGroups", "Groups");
            }
        }
    }
}