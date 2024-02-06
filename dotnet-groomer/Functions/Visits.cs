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
using System.Linq;
using dotnet_groomer.Models.User;

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

            List<VisitDto> response = null;
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.Customer)
                    .Include(v => v.VisitProducts)
                        .ThenInclude(vp => vp.Product)
                    .ToListAsync();

                response = visits.Select(visit =>
                {
                    UserDto userDto = null;
                    if (visit.CustomerId != null)
                    {
                        userDto = new UserDto()
                        {
                            Id = visit.Customer?.Id,
                            Email = visit.Customer?.Email,
                            Name = visit.Customer?.Name
                        };
                    }
                    
                    return new VisitDto
                    {
                        Id = visit.Id,
                        Title = visit.Title,
                        Start = visit.Start,
                        End = visit.End,
                        PaymentCleared = visit.PaymentCleared,
                        Price = visit.Price,
                        Products = visit?.VisitProducts.Select(vp => new ProductDto
                        {
                            Id = vp.Product.Id,
                            Name = vp.Product.Name,
                            Price = vp.Product.Price,
                            Time = vp.Product.Time,
                        }).ToList() ?? new(),
                        Customer = userDto,
                    };
                })
                .ToList();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new OkObjectResult(response);
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
                    VisitProducts = new List<VisitProduct>(),
                    CustomerId = data?.Customer?.Id ?? null,
                    Price = data.Price,
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

                visit.Title = data.Title ?? visit.Title;
                visit.Start = data.Start;
                visit.End = data.End;
                visit.AllDay = data.AllDay;
                visit.PaymentCleared = data.PaymentCleared;
                visit.Price = data.Price;

                visit.VisitProducts.Clear();

                if (data.ProductIds != null)
                {
                    var productIds = data.ProductIds.Select(x => x.Id);

                    var productsToAdd = await _context.Products
                        .Where(product => productIds.Contains(product.Id))
                        .ToListAsync();

                    foreach (var product in productsToAdd)
                    {
                        visit.VisitProducts.Add(new VisitProduct { ProductId = product.Id, VisitId = visit.Id });
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult(new VisitDto()
            {
                Title = visit.Title,
                Start = visit.Start,
                End = visit.End,
                AllDay = visit.AllDay,
                PaymentCleared = visit.PaymentCleared,
                Price = visit.Price,
                Products = visit.VisitProducts.Select(vp => new ProductDto
                {
                    Id = vp.Product.Id,
                    Name = vp.Product.Name,
                }).ToList(),
                Id = visit.Id,
            });
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
