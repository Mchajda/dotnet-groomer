using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models.Visit
{
    public class VisitProduct
    {
        [Column("visit_id")]
        public int VisitId { get; set; }

        public Visit Visit { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

    }
}
