using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace GroupUp.Controllers
{
    public class ReportsController : Controller
    {
        public readonly ApplicationDbContext _context;

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
            if (targetUser.AspNetIdentity.Id == User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            var viewModel = new UserReportViewModel()
            {
                UserReport = new UserReport()
                {
                    TargetUser = targetUser
                },
                ReportedUserId = targetUser?.UserId ?? -1
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
            var viewModel = new GroupReportViewModel()
            {
                GroupReport = new GroupReport()
                {
                    TargetGroup = targetGroup
                },
                ReportedGroupId = targetGroup?.GroupId ?? -1
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult PostUserReport(UserReportViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                int countOfErrors = 0;
                foreach (var value in ModelState.Values)
                {
                    countOfErrors += value.Errors.Count;
                    
                }

                // if only error is in the User field, we fix this error and add the user report.
                if (countOfErrors <= 1 && ModelState.Values.ElementAt(1).Errors.Count == 1)
                {
                    var targetUser = _context.Users.Include(u => u.AspNetIdentity)
                        .SingleOrDefault(u => u.UserId == viewModel.ReportedUserId);
                    viewModel.UserReport.TargetUser = targetUser;
                    _context.UserReports.Add(viewModel.UserReport);
                    _context.SaveChanges();
                    return RedirectToAction("UserGroups", "Groups");
                }
                return View("ReportUser", viewModel);
            }
            else
            {
                _context.UserReports.Add(viewModel.UserReport);
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
                int countOfErrors = 0;
                foreach (var value in ModelState.Values)
                {
                    countOfErrors += value.Errors.Count;

                }

                // if only error is in the User field, we fix this error and add the user report.
                if (countOfErrors <= 1 && ModelState.Values.ElementAt(1).Errors.Count == 1)
                {
                    var targetGroup = _context.Groups.SingleOrDefault(g => g.GroupId == viewModel.ReportedGroupId);
                    viewModel.GroupReport.TargetGroup = targetGroup;
                    _context.GroupReports.Add(viewModel.GroupReport);
                    _context.SaveChanges();
                    return RedirectToAction("UserGroups", "Groups");
                }
                return View("ReportUser", viewModel);
            }
            else
            {
                _context.GroupReports.Add(viewModel.GroupReport);
                _context.SaveChanges();
                return RedirectToAction("UserGroups", "Groups");
            }
        }
    }
}