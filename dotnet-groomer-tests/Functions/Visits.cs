using dotnet_groomer.Models.Visit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace dotnet_groomer_tests.Functions
{
    [TestClass]
    public class Visits
    {
        [TestMethod]
        public async Task GetVisits_ShouldReturnVisits_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithVisits(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Visits(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.GetVisits(request, logger) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var visits = result.Value as IEnumerable<Visit>;
            Assert.IsNotNull(visits);
            Assert.IsTrue(visits.Any());

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PostVisits_ShouldAddVisits_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Visits(dbContext);

            var visit = new Visit
            {
                Id = 1,
                Title = "Test Visit",
                Start = DateTime.Parse("2024-01-06T07:30:00+01:00"),
                End = DateTime.Parse("2024-01-06T08:00:00+01:00"),
                AllDay = false
            };

            // Act
            var result = await AddVisitsToDatabase(dbContext, function, logger, JsonSerializer.Serialize(visit)) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var returnedVisit = result.Value as Visit;
            Assert.IsNotNull(returnedVisit);
            Assert.AreEqual("Test Visit", returnedVisit.Title);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task UpdateVisits_ShouldReturnNewVisits_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithVisits(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Visits(dbContext);
            var request = new DefaultHttpContext().Request;

            var visit = new VisitRequestBody
            {
                Id = 1,
                Title = "Test Visit 2",
                Start = DateTime.Parse("2024-01-06T07:30:00+01:00"),
                End = DateTime.Parse("2024-01-06T08:00:00+01:00"),
                AllDay = false
            };

            // Act
            var result = await AddVisitsToDatabase(dbContext, function, logger, JsonSerializer.Serialize(visit), 1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var returnedVisit = result.Value as Visit;
            Assert.IsNotNull(returnedVisit);
            Assert.AreEqual("Test Visit 2", returnedVisit.Title);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task DeleteVisits_ShouldReturnEmptyDatabase_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithVisits(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Visits(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.DeleteVisit(request, logger, 1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var visits = result.Value as IEnumerable<Visit>;
            Assert.IsNull(visits);

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
        // TO DO: to be adjusted to visits
        private async static Task<ObjectResult> AddVisitsToDatabase(MyDbContext dbContext, dotnet_groomer.Functions.Visits function, ILogger logger, string userJson, int userId = -1)
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(userJson));

            if (userId == -1)
            {
                return await function.PostVisit(request, logger) as ObjectResult;
            }
            else
            {
                return await function.UpdateVisit(request, logger, userId) as ObjectResult;
            }
        }

        private static void PopulateDatabaseWithVisits(MyDbContext dbContext)
        {
            // Add users directly to dbContext for testing GetUsers
            dbContext.Visits.Add(new Visit
            {
                Id = 1,
                Title = "Test Visit",
                Start = DateTime.Parse("2024-01-06T07:30:00+01:00"),
                End = DateTime.Parse("2024-01-06T08:00:00+01:00"),
                AllDay = false
            });
            dbContext.SaveChanges();
        }
    }
}