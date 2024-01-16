using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dotnet_groomer
{
    public class GetProducts
    {
        [FunctionName("GetProducts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to fetch users from MySQL.");

            string connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");
            List<string> users = new List<string>();

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand("SELECT email FROM users", conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

            return new OkObjectResult(users);
        }
    }
}
