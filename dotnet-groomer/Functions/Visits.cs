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
using dotnet_groomer.Models.Visit;
using dotnet_groomer.Models;

namespace dotnet_groomer.Functions
{
    public class Visits
    {
        private readonly MyDbContext _context;

        public Visits(MyDbContext context)
        {
            _context = context;
        }

        [FunctionName("GetVisits")]
        public async Task<IActionResult> GetVisits(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to fetch visits from MySQL.");

            List<Visit> visits = null;
            try
            {
                visits = await _context.Visits.ToListAsync();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new OkObjectResult(visits);
        }

        [FunctionName("PostVisit")]
        public async Task<IActionResult> PostVisit(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<VisitRequestBody>(requestBody);

            Visit visit;
            try
            {
                visit = new Visit {
                    Title = data.Title,
                    Start = data.Start,
                    End = data.End,
                    AllDay = data.AllDay,
                    VisitProducts = new List<VisitProduct>()
                };

                if (data.ProductIds != null)
                {
                    foreach (var productId in data.ProductIds)
                    {
                        visit.VisitProducts.Add(new VisitProduct { ProductId = productId.Id });
                    }
                }

                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult(visit);
        }

        [FunctionName("UpdateVisit")]
        public async Task<IActionResult> UpdateVisit(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UpdateVisit/{visitId}")] HttpRequest req,
            ILogger log, int? visitId)
        {
            if (visitId == null)
            {
                return new NotFoundObjectResult("visit_id cannot be null");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<VisitRequestBody>(requestBody);

            Visit visit;
            try
            {
                visit = await _context.Visits
                                .Include(v => v.VisitProducts)
                                .FirstOrDefaultAsync(v => v.Id == visitId);

                visit.Title = data.Title;
                visit.Start = data.Start;
                visit.End = data.End;
                visit.AllDay = data.AllDay;
                visit.PaymentCleared = data.PaymentCleared;

                visit.VisitProducts.Clear();

                if (data.ProductIds != null)
                {
                    foreach (var productId in data.ProductIds)
                    {
                        visit.VisitProducts.Add(new VisitProduct { ProductId = productId.Id });
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult(visit);
        }

        [FunctionName("DeleteVisit")]
        public async Task<IActionResult> DeleteVisit(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteVisit/{visitId}")] HttpRequest req,
            ILogger log, int? visitId)
        {
            if (visitId == null)
            {
                return new NotFoundObjectResult("visit_id cannot be null");
            }

            Visit visit;
            try
            {
                visit = await _context.Visits.FindAsync(visitId);

                _context.Remove(visit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult("Visit was removed correctly");
        }
    }
}
