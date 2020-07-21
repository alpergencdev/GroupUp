using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GroupUp.Controllers;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;
using Moq;
using Xunit;

namespace GroupUp.Tests
{
    public class GroupsControllerTest
    {
        [Theory]
        [InlineData(-1, "test", "test", 5, "Test", "Test", "Test", true)] // Successful Create Test
        [InlineData(18, "Test", "TestEdit", 5, "Test", "Test", "Test", true)] // Successful Edit Test
        [InlineData(-1, "test", "test", 1, "Test", "Test", "Test", false)] // Unsuccessful Create Test
        [InlineData(9999, "Test", "TestEdit", 5, "Test", "Test", "Test", false)] // Unsuccessful Edit Test
        public void TestSaveGroup(int groupId, string title, string desc, int maxUsers, string city, string country, string continent, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() {CallBase = true};
            var contextMock = new Mock<ApplicationDbContext>() {CallBase = true};
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => "52a0d912-33dd-42fc-9b5b-959ed573e6dd"
            };

            var viewModel = new CreateGroupViewModel()
            {
                City = city,
                Country = country,
                Continent = continent,
                GroupId = groupId,
                DetailedDescription = desc,
                MaxUserCapacity = maxUsers,
                Title = title
            };
            if (expectingSuccess)
            {
                var result = (RedirectToRouteResult)controller.Save(viewModel);
                Assert.Equal("UserGroups", result.RouteValues["action"]);
            }
            else
            {
                var valContext = new ValidationContext(viewModel, null, null);
                var valResults = new List<ValidationResult>();
                if (Validator.TryValidateObject(viewModel, valContext, valResults, true))
                {
                    var result = controller.Save(viewModel);
                    Assert.True(result is HttpNotFoundResult);
                }
                else
                {
                    Assert.True(true);
                }
                
            }
        }

        [Theory]
        [InlineData(19, "e48d6b01-3dc8-4f11-a86c-c35affb09c0c", true, true)] // Successful Join
        [InlineData(19, "881bf243-d34d-49cb-b22b-cc3cd7a0fac5", false, false)] // Unsuccessful Join (Insufficient Security Level)
        [InlineData(-1, "e48d6b01-3dc8-4f11-a86c-c35affb09c0c", true, false)] // Unsuccessful Join (No such group)
        [InlineData(16, "e48d6b01-3dc8-4f11-a86c-c35affb09c0c", true, false)] // Unsuccessful Join (Closed group)
        [InlineData(19, "", true, false)] // Unsuccessful Join (No such user)
        [InlineData(19, "453a2901-150b-4211-84b1-a40ac2ba4a35", true, false)] // Unsuccessful Join (User already in group)
        public void TestJoinGroup(int groupId, string aspUserId, bool userIsSecLevel1, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            controllerContextMock.Setup(c => c.HttpContext.User.IsInRole("SecurityLevel1")).Returns(userIsSecLevel1);

            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => aspUserId
            };
            var result = controller.Join(groupId);
            if (expectingSuccess)
            {
                var redirectResult = (RedirectToRouteResult) result;
                Assert.Equal("Details", redirectResult.RouteValues["action"]);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult || result is ContentResult);
            }

        }

        [Fact]
        public void TestKickFromGroup()
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => "453a2901-150b-4211-84b1-a40ac2ba4a35"
            };
        }
    }

}