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
    public class Test
    {
        [FunctionName("Test")]
        public async Task<IActionResult> TestEndpoint(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Test")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to test connection.");        

            return new OkObjectResult("Connected.");
        }
    }
}
