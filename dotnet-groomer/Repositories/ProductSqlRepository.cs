using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnet_groomer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_groomer.Repositories
{
    public class ProductSqlRepository : IProductRepository
    {
        private readonly MyDbContext _context;

        public ProductSqlRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            List<ProductDto> products = null;
            try
            {
                var dbProducts = await _context.Products.ToListAsync();

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
                throw;
            }

            return products;
        }
    }
}
