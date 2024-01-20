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

namespace dotnet_groomer.Functions
{
    public class Users
    {
        private readonly MyDbContext _context;

        public Users(MyDbContext context)
        {
            _context = context;
        }

        [FunctionName("GetUsers")]
        public async Task<IActionResult> GetUsers(
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

        [FunctionName("PostUser")]
        public async Task<IActionResult> PostUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<User>(requestBody);

            User user;
            try
            {
                user = new User { Email = data.Email };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult(user);
        }

        [FunctionName("UpdateUser")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UpdateUser/{userId}")] HttpRequest req,
            ILogger log, int userId)
        {
            if (userId == null)
            {
                return new NotFoundObjectResult("user_id cannot be null");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<User>(requestBody);

            User user;
            try
            {
                user = await _context.Users.FindAsync(userId);

                user.Email = data.Email;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new OkObjectResult(user);
        }

        [FunctionName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteUser/{userId}")] HttpRequest req,
            ILogger log, int userId)
        {
            if (userId == null)
            {
                return new NotFoundObjectResult("user_id cannot be null");
            }

            User user;
            try
            {
                user = await _context.Users.FindAsync(userId);

                _context.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new OkObjectResult("User was removed correctly");
        }
    }
}
