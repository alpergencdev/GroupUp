using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
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
            string UserName = "rolestest";
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(UserName);

            var contextMock = new Mock<ApplicationDbContext>() {CallBase = true};
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object
            };
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserRoleStore = mockUserStore.As<IUserRoleStore<ApplicationUser>>();
            var userManager = new ApplicationUserManager(mockUserStore.Object);

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
    }


}