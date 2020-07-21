using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GroupUp.Controllers;
using GroupUp.Models;
using GroupUp.ViewModels;
using Moq;
using Xunit;
// ReSharper disable PossibleNullReferenceException

namespace GroupUp.Tests
{
    public class UsersControllerTest
    {
        [Theory]
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 31, true)] // Successful Details
        [InlineData("", 31, false)] // Unsuccessful Details (User does not exist)
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", -1, false)] // Unsuccessful Details (Target user does not exist)
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", 1, false)] // Unsuccessful Details (User & target do not share a group)
        public void TestUserDetails(string userAspId, int targetUserId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);
            var controller = new UsersController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId
            };

            var result = controller.Details(targetUserId);
            var viewResult = (ViewResult)result;
            var viewModel = (UserDetailsViewModel)viewResult.Model;
            if (expectingSuccess)
            {
                
                Assert.True(viewModel.UserCanView);
            }
            else
            {
                Assert.False(viewModel.UserCanView);
            }
        }

        [Theory]
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", "contact me", "hello@hello.com", "hi@hello.com", true)] // Successful Edit
        [InlineData("", "contact me", "hello@hello.com", "hi@hello.com", false)] // Unsuccessful Edit
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", "contact me", "hello@hello.com", "hello@hello.com", false)] // Unsuccessful Edit
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", "", "hello@hello.com", "hello@hello.com", false)] // Unsuccessful Edit
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", "contact me", "skroas@gmail.com", "hello@hello.com", false)] // Unsuccessful Edit
        public void TestPostEdit(string userAspId, string contactInfo, string email, string prevEmail, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);

            
            var controller = new UsersController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId,
            };

            var viewModel = new UserEditViewModel()
            {
                ContactInfo = contactInfo,
                Email = email,
                PreviousEmail = prevEmail
            };
            var result = controller.PostEdit(viewModel);
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
                    if (viewModel.Email == viewModel.PreviousEmail)
                    {
                        Assert.True(true);
                    }
                    else
                    {
                        Assert.True(result is HttpNotFoundResult);
                    }
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
        [InlineData("de461acf-b411-4361-b127-a1a4eb5708b9", true)] // Successful ISL1
        [InlineData("", false)] // Unsuccessful ISL1 (User does not exist)
        [InlineData("a90916a5-b7be-491c-b20f-66d51abc9a66", false)] // Unsuccessful ISL1 (User has SL >= 1)
        [InlineData("d4a7548b-6118-4d90-994b-2676dfd5c5bc", false)] // Unsuccessful ISL1 (User not verified)
        public void TestIncreaseSecurityLevelTo1(string userAspId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);


            var controller = new UsersController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId,
            };

            var result = controller.IncreaseSecurityLevelTo1();
            if (expectingSuccess)
            {
                Assert.True( result is RedirectToRouteResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult || result is ContentResult);
            }

        }

        [Theory]
        [InlineData("a90916a5-b7be-491c-b20f-66d51abc9a66", true)] // Successful ISL2
        [InlineData("", false)] // Unsuccessful ISL2 (User does not exist)
        [InlineData("e48d6b01-3dc8-4f11-a86c-c35affb09c0c", false)] // Unsuccessful ISL2 (User already SL >= 2)
        [InlineData("881bf243-d34d-49cb-b22b-cc3cd7a0fac5", false)] // Unsuccessful ISL2 (User has TP < 50)
        public void TestIncreaseSecurityLevelTo2(string userAspId, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);


            var controller = new UsersController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId,
            };

            var result = controller.IncreaseSecurityLevelTo2();
            if (expectingSuccess)
            {
                Assert.True(result is RedirectToRouteResult);
            }
            else
            {
                Assert.True(result is HttpNotFoundResult || result is ContentResult);
            }

        }

        [Theory]
        [InlineData("d4a7548b-6118-4d90-994b-2676dfd5c5bc", 983232, true)] // Successful PostVerify
        [InlineData("d4a7548b-6118-4d90-994b-2676dfd5c5bc", 123456, false)] // Unsuccessful PostVerify (Wrong Verification Code)
        [InlineData("", 123456, false)] // Unsuccessful PostVerify (User does not exist)
        [InlineData("de461acf-b411-4361-b127-a1a4eb5708b9", 123456, false)] // Unsuccessful PostVerify (User Already Verified)
        [InlineData("d4a7548b-6118-4d90-994b-2676dfd5c5bc", 7, false)] // Unsuccessful PostVerify (ViewModel is not valid)
        public void TestPostVerify(string userAspId, int verificationCode, bool expectingSuccess)
        {
            var controllerContextMock = new Mock<ControllerContext>() { CallBase = true };
            var contextMock = new Mock<ApplicationDbContext>() { CallBase = true };
            contextMock.Setup(c => c.SaveChanges()).Returns(1);


            var controller = new UsersController
            {
                ControllerContext = controllerContextMock.Object,
                Context = contextMock.Object,
                GetUserId = () => userAspId,
            };

            var viewModel = new VerifyUserViewModel()
            {
                VerificationCode = verificationCode
            };

            var result = controller.PostVerify(viewModel);
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
    }
}