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

namespace dotnet_groomer
{
    public class GetProducts
    {
        private readonly MyDbContext _context;

        public GetProducts(MyDbContext context)
        {
            _context = context;
        }

        [FunctionName("GetProducts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to fetch users from MySQL.");

            List<User> users = null;
            try
            {
                users = await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new OkObjectResult(users);
        }
    }
}
