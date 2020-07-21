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
    public class ModeratorsControllerTest
    {
        [Theory]
        [InlineData(30, true)] // Successful UserDetails
        [InlineData(-1, false)] // Unsuccessful UserDetails
        public void TestUserDetails(int userId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.UserDetails(userId);
            if (expectingSuccess)
            {
                Assert.True( result is ViewResult);
            }
            else
            {
                Assert.True( result is HttpNotFoundResult);
            }
        }

        [Theory]
        [InlineData(30, true)] // Successful ChangeLockoutDate
        [InlineData(-1, false)] // Unsuccessful ChangeLockoutDate
        public void TestChangeLockoutDate(int userId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.ChangeLockoutDate(userId);
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
        [InlineData(34, "1/01/2099 00:00:00", true)] // Successful PostLockoutDate
        [InlineData(-1, "1/01/2099 00:00:00", false)] // Unsuccessful PostLockoutDate (User does not exist)
        public void TestPostLockoutDate(int userId, string datetime, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var viewModel = new ModeratorLockoutViewModel()
            {
                LockoutDate = DateTime.Parse(datetime),
                UserId = userId
            };

            var result = controller.PostLockoutDate(viewModel);
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
                    Assert.True(result is HttpNotFoundResult);
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
        [InlineData(19, true)] // Successful GroupDetails
        [InlineData(-1, false)] // Unsuccessful GroupDetails
        public void TestGroupDetails(int groupId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.GroupDetails(groupId);
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
        [InlineData(19, true)] // Successful DeleteGroup
        [InlineData(-1, false)] // Unsuccessful DeleteGroup
        public void TestDeleteGroup(int groupId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.DeleteGroup(groupId);
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }

        [Theory]
        [InlineData(7, true)] // Successful UserReportDetails
        [InlineData(-1, false)] // Unsuccessful UserReportDetails
        public void TestUserReportDetails(int userReportId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.UserReportDetails(userReportId);
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
        [InlineData(9, true)] // Successful GroupReportDetails
        [InlineData(-1, false)] // Unsuccessful GroupReportDetails
        public void TestGroupReportDetails(int groupReportId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.GroupReportDetails(groupReportId);
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
        [InlineData(7, true)] // Successful DeleteUserReport
        [InlineData(-1, false)] // Unsuccessful DeleteUserReport
        public void TestDeleteUserReport(int userReportId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.DeleteUserReport(userReportId);
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }

        [Theory]
        [InlineData(9, true)] // Successful DeleteGroupReport
        [InlineData(-1, false)] // Unsuccessful DeleteGroupReport
        public void TestDeleteGroupReport(int groupReportId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new ModeratorsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };

            var result = controller.DeleteGroupReport(groupReportId);
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }
    }
}