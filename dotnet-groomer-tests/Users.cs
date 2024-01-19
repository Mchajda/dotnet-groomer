using dotnet_groomer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using dotnet_groomer.Functions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace dotnet_groomer_tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task PostUser_ShouldAddUser_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var logger = Mock.Of<ILogger>();
            var function = new Users(dbContext);
            var userJson = "{\"email\":\"test@example.com\"}";

            // Act
            var result = await AddUserToDatabase(dbContext, function, logger, userJson) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            var returnedUser = result.Value as User;
            Assert.IsNotNull(returnedUser);
            Assert.AreEqual("test@example.com", returnedUser.Email);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetUsers_ShouldReturnUser_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithUsers(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new Users(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.GetUsers(request, logger) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var users = result.Value as IEnumerable<User>;
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        private static MyDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MyDbContext(options);
        }

        private async static Task<ObjectResult> AddUserToDatabase(MyDbContext dbContext, Users function, ILogger logger, string userJson)
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(userJson));

            return await function.PostUser(request, logger) as ObjectResult;
        }

        private static void PopulateDatabaseWithUsers(MyDbContext dbContext)
        {
            // Add users directly to dbContext for testing GetUsers
            dbContext.Users.Add(new User { Email = "john@doe.com" });
            dbContext.SaveChanges();
        }
    }
}