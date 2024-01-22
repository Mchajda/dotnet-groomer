using dotnet_groomer.Models;
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
    public class VisitsAnalytics
    {
        [TestMethod]
        public async Task GetVisitsForWeek_ShouldReturnVisits_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithVisits(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.VisitsAnalytics(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.GetVisitsForWeek(request, logger, 2024, 1) as ObjectResult;
            var visits = result.Value as IEnumerable<Visit>;

            var result2 = await function.GetVisitsForWeek(request, logger, 2024, 2) as ObjectResult;
            var visits2 = result2.Value as IEnumerable<Visit>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            Assert.IsNotNull(visits);
            Assert.IsTrue(visits.Any());

            Assert.AreEqual(0, visits2.Count());
            Assert.IsFalse(visits2.Any());

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

        private static void PopulateDatabaseWithVisits(MyDbContext dbContext)
        {
            // Add users directly to dbContext for testing GetUsers
            dbContext.Visits.Add(new Visit
            {
                Id = 1,
                Title = "Test Visit",
                Start = "2024-01-06T07:30:00+01:00",
                End = "2024-01-06T08:00:00+01:00",
                AllDay = false
            });
            dbContext.SaveChanges();
        }
    }
}