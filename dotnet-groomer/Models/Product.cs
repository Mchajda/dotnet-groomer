using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Price { get; set; }
        public int Time { get; set; }
    }
}
