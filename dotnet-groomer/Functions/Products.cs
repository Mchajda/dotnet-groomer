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
using dotnet_groomer.Models;
using System.Linq;
using dotnet_groomer.Repositories.Interfaces;

namespace dotnet_groomer.Functions
{
    public class Products
    {
        private readonly MyDbContext _context;
        private readonly IProductRepository _repository;

        public Products(MyDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _repository = productRepository;
        }

        [FunctionName("GetProducts")]
        public async Task<IActionResult> GetProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to fetch products from MySQL.");

            List<ProductDto> products = null;
            try
            {
                var dbProducts = await _repository.GetProducts();

                products = dbProducts.Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Time = product.Time,
                    Price = product.Price,
                }).ToList();
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }

            return new OkObjectResult(products);
        }

        [FunctionName("PostProduct")]
        public async Task<IActionResult> PostProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Product>(requestBody);

            Product product;
            try
            {
                product = new Product { 
                    Name = data.Name,
                    Price = data.Price,
                    Time = data.Time,
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult(product);
        }

        [FunctionName("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UpdateProduct/{productId}")] HttpRequest req,
            ILogger log, int? productId)
        {
            if (productId == null)
            {
                return new NotFoundObjectResult("product_id cannot be null");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Product>(requestBody);

            Product product;
            try
            {
                product = await _context.Products.FindAsync(productId);

                product.Name = data.Name;
                product.Price = data.Price;
                product.Time = data.Time;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult(product);
        }

        [FunctionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteProduct/{productId}")] HttpRequest req,
            ILogger log, int? productId)
        {
            if (productId == null)
            {
                return new NotFoundObjectResult("product_id cannot be null");
            }

            Product product;
            try
            {
                product = await _context.Products.FindAsync(productId);

                _context.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message} // {ex.InnerException.Message}");
            }

            return new OkObjectResult("Product was removed correctly");
        }
    }
}
