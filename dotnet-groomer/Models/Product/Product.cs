using dotnet_groomer.Models.Visit;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace dotnet_groomer.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Time { get; set; }
        public List<VisitProduct> VisitProducts { get; set; }
    }
}
