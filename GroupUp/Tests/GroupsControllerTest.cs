using System.Threading.Tasks;
using System.Web.Mvc;
using GroupUp.Controllers;
using GroupUp.Models;
using GroupUp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GroupUp.Tests
{
    [TestClass]
    public class GroupsControllerTest
    {
        [TestMethod]
        public void TestSaveGroup()
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
                City = "Test",
                Country = "Test",
                Continent = "Test",
                GroupId = -1,
                DetailedDescription = "hope this works",
                MaxUserCapacity = 5,
                Title = "automated test"
            };
            var result = (RedirectToRouteResult) controller.Save(viewModel);
            Assert.AreEqual("UserGroups", result.RouteValues["action"]);
        }
    }


}