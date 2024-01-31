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
                    int dayIndex = group.Key == 0 ? 6 : group.Key - 1;
                    groupedVisits[dayIndex] = group.ToList();
                }

                return new OkObjectResult(groupedVisits);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }

        [FunctionName("GetIncomeForWeek")]
        public async Task<IActionResult> GetIncomeForWeek(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetIncomeForWeek/{year}/{weekNumber}")] HttpRequest req,
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
                    .ToDictionary(day => day, day => new int());

                foreach (var group in visitsInWeek.GroupBy(visit => (int)visit.Start.DayOfWeek))
                {
                    int sum = 0;
                    foreach (var item in group.ToList())
                    {
                        sum += item.Price;
                    }

                    int dayIndex = group.Key == 0 ? 6 : group.Key - 1;
                    groupedVisits[dayIndex] = sum;
                }

                return new OkObjectResult(groupedVisits);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }

        [FunctionName("GetVisitsForMonth")]
        public async Task<IActionResult> GetVisitsForMonth(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVisitsForMonth/{year}/{month}")] HttpRequest req,
            ILogger log, int year, int month)
        {
            log.LogInformation("C# HTTP trigger function processed a request to fetch visits from MySQL.");
            string withoutEmpty = req.Query["leaveEmpty"];

            try
            {
                var (monthStart, monthEnd) = GetStartAndEndDateOfMonth(year, month);

                var visitsInMonth = await _context.Visits
                    .Include(visit => visit.VisitProducts)
                        .ThenInclude(visitProduct => visitProduct.Product)
                    .Where(visit => visit.Start >= monthStart && visit.Start < monthEnd)
                    .ToListAsync();

                DateTimeOffset iteratorDate = monthEnd;
                List<VisitsForMonthResponseItem> visitsGroupped = new();

                while (iteratorDate >= monthStart)
                {
                    var dayVisits = visitsInMonth
                        .Where(visit => visit.Start >= iteratorDate && visit.Start < iteratorDate.AddDays(1))
                        .Select(visit => new VisitDto
                        {
                            Id = visit.Id,
                            Title = visit.Title,
                            Start = visit.Start,
                            End = visit.End,
                            PaymentCleared = visit.PaymentCleared,
                            Products = visit.VisitProducts.Select(vp => new ProductDto
                            {
                                Id = vp.Product.Id,
                                Name = vp.Product.Name,
                            }).ToList()
                        })
                        .ToList();

                    if (withoutEmpty == "true")
                    {
                        if (dayVisits.Count > 0)
                        {
                            visitsGroupped.Add(new VisitsForMonthResponseItem
                            {
                                Date = iteratorDate.ToString("yyyy-MM-dd"),
                                Visits = dayVisits
                            });
                        }
                    }
                    else
                    {
                        visitsGroupped.Add(new VisitsForMonthResponseItem
                        {
                            Date = iteratorDate.ToString("yyyy-MM-dd"),
                            Visits = dayVisits
                        });
                    }

                    iteratorDate = iteratorDate.AddDays(-1);
                }

                return new OkObjectResult(visitsGroupped);

            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }

        public class VisitsForMonthResponseItem
        {
            public string Date { get; set; }
            public List<VisitDto> Visits { get; set; }
        }

        // TO DO: move it to utils
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

        public (DateTime, DateTime) GetStartAndEndDateOfMonth(int year, int month)
        {
            var lastDayOfMonth = DateTime.DaysInMonth(year, month);
            return (DateTime.Parse($"{year}-{month}-01"), DateTime.Parse($"{year}-{month}-{lastDayOfMonth}"));
        }
    }
}
