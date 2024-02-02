using dotnet_groomer.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace dotnet_groomer_tests.Functions
{
    [TestClass]
    public class Users
    {
        [TestMethod]
        public async Task GetUsers_ShouldReturnUser_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithUsers(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Users(dbContext);
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

        [TestMethod]
        public async Task PostUser_ShouldAddUser_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Users(dbContext);
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
        public async Task UpdateUser_ShouldReturnNewUser_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithUsers(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Users(dbContext);
            var request = new DefaultHttpContext().Request;
            var userJson = "{\"email\":\"test@example.com\"}";

            // Act
            var result = await AddUserToDatabase(dbContext, function, logger, userJson, 1) as ObjectResult;

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
        public async Task DeleteUser_ShouldReturnEmptyDatabase_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithUsers(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Users(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.DeleteUser(request, logger, 1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var users = result.Value as IEnumerable<User>;
            Assert.IsNull(users);

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

        private async static Task<ObjectResult> AddUserToDatabase(MyDbContext dbContext, dotnet_groomer.Functions.Users function, ILogger logger, string userJson, int userId = -1)
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(userJson));

            if (userId == -1)
            {
                return await function.PostUser(request, logger) as ObjectResult;
            }
            else
            {
                return await function.UpdateUser(request, logger, userId) as ObjectResult;
            }
        }

        private static void PopulateDatabaseWithUsers(MyDbContext dbContext)
        {
            // Add users directly to dbContext for testing GetUsers
            dbContext.Users.Add(new User { Id = 1, Email = "john@doe.com" });
            dbContext.SaveChanges();
        }
    }
}