using dotnet_groomer.Models.Visit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;
using static dotnet_groomer.Functions.VisitsAnalytics;

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
            var visits = result?.Value as Dictionary<int, List<Visit>>;

            var result2 = await function.GetVisitsForWeek(request, logger, 2024, 3) as ObjectResult;
            var visits2 = result2?.Value as Dictionary<int, List<Visit>>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            Assert.IsNotNull(visits);
            Assert.IsTrue(visits.Any());

            Assert.AreEqual(0, visits2[0].Count);
            Assert.AreEqual(1, visits[6].Count);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetIncomeForWeek_ShouldReturnIncome_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithVisits(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.VisitsAnalytics(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.GetIncomeForWeek(request, logger, 2024, 1) as ObjectResult;
            var visits = result?.Value as Dictionary<int, int>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            Assert.IsNotNull(visits);
            Assert.IsTrue(visits.Any());
            Assert.AreEqual(200, visits[6]);
            Assert.AreEqual(400, visits[5]);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetVisitsForMonth_ShouldReturnVisits_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithVisits(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.VisitsAnalytics(dbContext);
            var request = new DefaultHttpContext().Request;
            request.QueryString.Add("leaveEmpty", "true");

            // Act
            var result = await function.GetVisitsForMonth(request, logger, 2024, 1) as ObjectResult;
            var visits = result?.Value as List<VisitsForMonthResponseItem>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            Assert.IsNotNull(visits);
            Assert.AreEqual(2, visits[25].Visits.Count);

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
            dbContext.Visits.AddRange(new Visit
            {
                Id = 1,
                Title = "Test Visit",
                Start = DateTime.Parse("2024-01-06T07:30:00+01:00"),
                End = DateTime.Parse("2024-01-06T08:00:00+01:00"),
                AllDay = false,
                Price = 200
            },
            new Visit
            {
                Id = 2,
                Title = "Test Visit",
                Start = DateTime.Parse("2024-01-06T09:30:00+01:00"),
                End = DateTime.Parse("2024-01-06T10:00:00+01:00"),
                AllDay = false,
                Price = 200
            },
            new Visit
            {
                Id = 3,
                Title = "Test Visit",
                Start = DateTime.Parse("2024-01-07T07:30:00+01:00"),
                End = DateTime.Parse("2024-01-07T08:00:00+01:00"),
                AllDay = false,
                Price = 200
            });
            dbContext.SaveChanges();
        }
    }
}