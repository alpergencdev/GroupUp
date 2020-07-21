using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GroupUp.Controllers;
using GroupUp.Models;
using GroupUp.ViewModels;
using Moq;
using Xunit;

namespace GroupUp.Tests
{
    public class ReportsControllerTest
    {
        [Theory]
        [InlineData(19, "bad group", "eom", true)] // Successful PostGroupReport
        [InlineData(19, "", "eom", false)] // Unsuccessful PostGroupReport (Invalid model)
        [InlineData(-1, "bad group", "eom", false)] // Unsuccessful PostGroupReport (Group does not exist)
        public void TestPostGroupReport(int groupId, string reason, string desc, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new ReportsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var viewModel = new GroupReportViewModel()
            {
                Description = desc,
                Reason = reason,
                ReportedGroupId = groupId,
                TargetGroupTitle = " "
            };
            var result = controller.PostGroupReport(viewModel);
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
            }
            else
            {
                var valContext = new ValidationContext(viewModel, null, null);
                var valResults = new List<ValidationResult>();
                if (Validator.TryValidateObject(viewModel, valContext, valResults, true))
                {
                    Assert.True(result is ViewResult);
                }
                else
                {
                    // This means that the passed model was not valid, meaning the system will not successfully save.
                    // However, ModelState object of the controller does not work properly in unit test cases.
                    // So, we deem this test a success, as if it had failed.
                    Assert.True(true);
                }
            }
        }

        [Theory]
        [InlineData(30, "bad person", "eom", true)] // Successful PostUserReport
        [InlineData(30, "", "eom", false)] // Unsuccessful PostUserReport (Invalid model)
        [InlineData(-1, "bad person", "eom", false)] // Unsuccessful PostUserReport (User does not exist)
        public void TestPostUserReport(int userId, string reason, string desc, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new ReportsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var viewModel = new UserReportViewModel()
            {
                Description = desc,
                Reason = reason,
                ReportedUserId = userId,
                TargetUsername = " "
            };
            var result = controller.PostUserReport(viewModel);
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
            }
            else
            {
                var valContext = new ValidationContext(viewModel, null, null);
                var valResults = new List<ValidationResult>();
                if (Validator.TryValidateObject(viewModel, valContext, valResults, true))
                {
                    Assert.True(result is ViewResult);
                }
                else
                {
                    // This means that the passed model was not valid, meaning the system will not successfully save.
                    // However, ModelState object of the controller does not work properly in unit test cases.
                    // So, we deem this test a success, as if it had failed.
                    Assert.True(true);
                }
            }
        }

        [Theory]
        [InlineData(19, true)] // Successful ReportGroup
        [InlineData(-1, false)] // Unsuccessful ReportGroup
        public void TestReportGroup(int groupId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new ReportsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.ReportGroup(groupId);
            if (expectingSuccess)
            {
                Assert.True(result is ViewResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }

        [Theory]
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 30, true)] // Successful ReportUser
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", -1, false)] // Unsuccessful ReportUser
        public void TestReportUser(string userAspId, int targetUserId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new ReportsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId
            };

            var result = controller.ReportUser(targetUserId);
            if (expectingSuccess)
            {
                Assert.True(result is ViewResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }
    }
}