using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using dotnet_groomer.Models;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using System.Linq;


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
                var calendar = ci.Calendar;
                var firstDayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;

                visits = await _context.Visits
                    .Where(visit =>
                        calendar.GetYear(DateTime.Parse(visit.Start)) == year &&
                        calendar.GetWeekOfYear(DateTime.Parse(visit.Start), CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek) == weekNumber)
                    .ToListAsync();

                return new OkObjectResult(visits);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }
    }
}
