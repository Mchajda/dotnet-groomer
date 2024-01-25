using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using System.Linq;
using dotnet_groomer.Models.Visit;

namespace dotnet_groomer.Functions
{
    public class VisitsAnalytics
    {
        private readonly MyDbContext _context;

        public VisitsAnalytics(MyDbContext context)
        {
            _context = context;
        }

        [FunctionName("GetVisitsForWeek")]
        public async Task<IActionResult> GetVisitsForWeek(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVisitsForWeek/{year}/{weekNumber}")] HttpRequest req,
            ILogger log, int year, int weekNumber)
        {
            log.LogInformation("C# HTTP trigger function processed a request to fetch visits from MySQL.");

            List<Visit> visits = null;
            try
            {
                var ci = CultureInfo.InvariantCulture;
                var (weekStart, weekEnd) = GetStartAndEndDateOfWeek(year, weekNumber, ci);

                var visitsInWeek = await _context.Visits
                    .Where(visit => visit.Start >= weekStart && visit.Start < weekEnd)
                    .ToListAsync();

                var groupedVisits = Enumerable.Range(0, 7)
                    .ToDictionary(day => day, day => new List<Visit>());

                foreach (var group in visitsInWeek.GroupBy(visit => (int)visit.Start.DayOfWeek))
                {
                    groupedVisits[group.Key] = group.ToList();
                }

                return new OkObjectResult(groupedVisits);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }

        public (DateTime, DateTime) GetStartAndEndDateOfWeek(int year, int weekNumber, CultureInfo ci)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);

            var cal = ci.Calendar;
            var firstWeek = cal.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekNumber;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var resultStart = firstMonday.AddDays(weekNum * 7);
            var resultEnd = resultStart.AddDays(7).AddTicks(-1);

            return (resultStart, resultEnd);
        }
    }
}
