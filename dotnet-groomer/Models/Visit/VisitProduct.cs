using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models.Visit
{
    public class VisitProduct
    {
        [Column("visit_id")]
        public int VisitId { get; set; }
        [JsonIgnore]
        public Visit Visit { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
