using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_groomer.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> GetProducts();
    }
}
