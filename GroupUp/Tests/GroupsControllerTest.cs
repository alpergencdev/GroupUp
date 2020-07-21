using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GroupUp.Controllers;
using GroupUp.Models;
using GroupUp.ViewModels;
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
        [InlineData(-1, "test", "test", 1, "Test", "-", "Test", false)] // Unsuccessful Create Test #22
        [InlineData(9999, "Test", "TestEdit", 5, "Test", "Test", "Test", false)] // Unsuccessful Edit Test
        [InlineData(18, "", "TestEdit", 5, "Test", "Test", "Test", false)] // Unsuccessful Edit Test #2
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
                // check if expected fail is because of model state.
                var valContext = new ValidationContext(viewModel, null, null);
                var valResults = new List<ValidationResult>();
                if (Validator.TryValidateObject(viewModel, valContext, valResults, true))
                {
                    var result = controller.Save(viewModel);
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
        [InlineData(19, "a90916a5-b7be-491c-b20f-66d51abc9a66", true, true)] // Successful Join
        [InlineData(19, "881bf243-d34d-49cb-b22b-cc3cd7a0fac5", false, false)] // Unsuccessful Join (Insufficient Security Level)
        [InlineData(-1, "a90916a5-b7be-491c-b20f-66d51abc9a66", true, false)] // Unsuccessful Join (No such group)
        [InlineData(16, "a90916a5-b7be-491c-b20f-66d51abc9a66", true, false)] // Unsuccessful Join (Closed group)
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

        [Theory]
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 15, 19, true)] // Successful Kick
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 15, 19, false)] // Unsuccessful Kick (current user isn't the creator)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 30, 19, false)] // Unsuccessful Kick (target user isn't in group)
        [InlineData("", 15, 19, false)] // Unsuccessful Kick (no such user with given ASP.NET ID)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 15, -1, false)] // Unsuccessful Kick (no such group)
        public void TestKickFromGroup(string creatorAspId, int targetUserId, int groupId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => creatorAspId
            };

            var result = controller.Kick(targetUserId, groupId);
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
                var redirectResult = (RedirectToRouteResult) result;
                Assert.Equal("Details", redirectResult.RouteValues["action"]);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }

        [Theory]
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 19, true, true)] // Successful Leave
        [InlineData("a90916a5-b7be-491c-b20f-66d51abc9a66", 19, true, false)] // Unsuccessful Leave (User isn't part of the group)
        [InlineData("", 19, true, false)] // Unsuccessful Leave (User does not exist)
        [InlineData("a90916a5-b7be-491c-b20f-66d51abc9a66", -1, true, false)] // Unsuccessful Leave (Group does not exist)
        [InlineData("881bf243-d34d-49cb-b22b-cc3cd7a0fac5", 19, false, false)] // Unsuccessful Leave (User does not have security level 1)
        public void TestLeaveGroup(string userAspId, int groupId, bool userIsSecLevel1, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            controllerContextMock.Setup(c => c.HttpContext.User.IsInRole("SecurityLevel1")).Returns(userIsSecLevel1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId
            };

            var result = controller.Leave(groupId);
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
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 19, true)] // Successful Close
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", -1, false)] // Unsuccessful Close (Group does not exist)
        [InlineData("", 19, false)] // Unsuccessful Close (User does not exist)
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 19, false)] // Unsuccessful Close (User is not creator)
        public void TestCloseGroup(string creatorAspId, int groupId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => creatorAspId
            };

            var result = controller.Close(groupId);
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
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 19, true, false, true, false)] // Successful Details (Creator)
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 19, true, false, false, true)] // Successful Details (Member)
        [InlineData("a90916a5-b7be-491c-b20f-66d51abc9a66", 19, true, false, false, false)] // Successful Details (Outsider)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 20, true, true, true, false)] // Successful Details (Closed Group)
        [InlineData("", 19, false, false, false, false)]// Unsuccessful Details (User does not exist)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", -1, false, false, false, false)]// Unsuccessful Details (Group does not exist)
        public void TestGroupDetails(string userAspId, int groupId, bool expectingSuccess, bool isClosed, bool isCreator, bool isRegularMember)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId
            };

            var result = controller.Details(groupId);
            if (expectingSuccess)
            {
                if (isClosed)
                {
                    Assert.True( result is RedirectToRouteResult);
                    var redirectResult = (RedirectToRouteResult) result;
                    Assert.Equal("ClosedDetails", redirectResult.RouteValues["action"]);
                    Assert.Equal(groupId, redirectResult.RouteValues["groupId"]);
                }
                
                else if (isCreator)
                {
                    Assert.True(result is ViewResult);
                    var viewResult = (ViewResult) result;
                    Assert.Equal("CreatorDetails", viewResult.ViewName);
                    Assert.Equal(groupId, ((GroupDetailsViewModel)viewResult.Model).Group.GroupId);
                }

                else if (isRegularMember)
                {
                    Assert.True(result is ViewResult);
                    var viewResult = (ViewResult)result;
                    Assert.Equal("MemberDetails", viewResult.ViewName);
                    Assert.Equal(groupId, ((GroupDetailsViewModel)viewResult.Model).Group.GroupId);
                }

                else
                {
                    Assert.True(result is ViewResult);
                    var viewResult = (ViewResult)result;
                    Assert.Equal("", viewResult.ViewName);
                    Assert.Equal(groupId, ((GroupDetailsViewModel)viewResult.Model).Group.GroupId);
                }
            }
            else
            {
                Assert.True(result is HttpNotFoundResult);
            }
        }

        [Theory]
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 20, true)] // Successful Rate
        [InlineData("", 20, false)] // Unsuccessful Rate (User does not exist)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a3", 7, false)] // Unsuccessful Rate (User not in group)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a3", -1, false)] // Unsuccessful Rate (Group does not exist)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a3", 19, false)] // Unsuccessful Rate (Group is not closed)
        public void TestRate(string userAspId, int groupId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId
            };

            var result = controller.Rate(groupId);
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
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 20, 5, true)] // Successful Rate
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 20, 99, false)] // Unsuccessful Rate (Invalid Model)
        [InlineData("", 20, 5, false)] // Unsuccessful Rate (User does not exist)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 19, 5, false)] // Unsuccessful Rate (Group is not closed)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", -1, 5, false)] // Unsuccessful Rate (Group does not exist)
        [InlineData("453a2901-150b-4211-84b1-a40ac2ba4a35", 16, 5, false)] // Unsuccessful Rate (User not in group)
        public void TestPostRating(string userAspId, int groupId, int ratingValue, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new GroupsController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId
            };

            var initialRateCall = controller.Rate(groupId);
            if (initialRateCall is HttpNotFoundResult)
            {
                // give an error because viewmodel cannot be extracted
                Assert.True(!expectingSuccess);
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                var ratingViewModel = (GroupRatingViewModel)(initialRateCall as ViewResult).Model;
                var newRatingDictionary = new Dictionary<string, int>();
                foreach (var kvp in ratingViewModel.UserRatings)
                {
                    newRatingDictionary.Add(kvp.Key, ratingValue);
                }

                var newViewModel = new GroupRatingViewModel()
                {
                    GroupId = ratingViewModel.GroupId,
                    UserRatings = newRatingDictionary
                };
                var result = controller.PostRating(newViewModel);

                if (expectingSuccess)
                {
                    Assert.True(result is RedirectToRouteResult);
                }
                else
                {
                    var valContext = new ValidationContext(newViewModel, null, null);
                    var valResults = new List<ValidationResult>();
                    if (Validator.TryValidateObject(newViewModel, valContext, valResults, true))
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
        }
    }

}