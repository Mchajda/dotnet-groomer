using dotnet_groomer.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace dotnet_groomer_tests.Functions
{
    [TestClass]
    public class Products
    {
        [TestMethod]
        public async Task GetProducts_ShouldReturnProducts_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithProducts(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Products(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.GetProducts(request, logger) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var products = result.Value as IEnumerable<Product>;
            Assert.IsNotNull(products);
            Assert.IsTrue(products.Any());

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task PostProducts_ShouldAddProducts_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Products(dbContext);

            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 5000,
                Time = 60
            };

            // Act
            var result = await AddProductsToDatabase(dbContext, function, logger, JsonSerializer.Serialize(product)) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var returnedProduct = result.Value as Product;
            Assert.IsNotNull(returnedProduct);
            Assert.AreEqual("Test Product", returnedProduct.Name);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task UpdateProducts_ShouldReturnNewProducts_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithProducts(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Products(dbContext);
            var request = new DefaultHttpContext().Request;

            var product = new Product
            {
                Id = 1,
                Name = "Test Product 2",
                Price = 5000,
                Time = 60
            };

            // Act
            var result = await AddProductsToDatabase(dbContext, function, logger, JsonSerializer.Serialize(product), 1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var returnedProduct = result.Value as Product;
            Assert.IsNotNull(returnedProduct);
            Assert.AreEqual("Test Product 2", returnedProduct.Name);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task DeleteProducts_ShouldReturnEmptyDatabase_WhenCalledWithValidData()
        {
            // Arrange
            var dbContext = CreateDbContext();
            PopulateDatabaseWithProducts(dbContext);
            var logger = Mock.Of<ILogger>();
            var function = new dotnet_groomer.Functions.Products(dbContext);
            var request = new DefaultHttpContext().Request;

            // Act
            var result = await function.DeleteProduct(request, logger, 1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var products = result.Value as IEnumerable<Product>;
            Assert.IsNull(products);

            // Cleanup
            dbContext.Database.EnsureDeleted();
        }

        private static MyDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MyDbContext(options);
        }
        // TO DO: to be adjusted to products
        private async static Task<ObjectResult> AddProductsToDatabase(MyDbContext dbContext, dotnet_groomer.Functions.Products function, ILogger logger, string userJson, int userId = -1)
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(userJson));

            if (userId == -1)
            {
                return await function.PostProduct(request, logger) as ObjectResult;
            }
            else
            {
                return await function.UpdateProduct(request, logger, userId) as ObjectResult;
            }
        }

        private static void PopulateDatabaseWithProducts(MyDbContext dbContext)
        {
            // Add users directly to dbContext for testing GetUsers
            dbContext.Products.Add(new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 5000,
                Time = 60
            });
            dbContext.SaveChanges();
        }
    }
}