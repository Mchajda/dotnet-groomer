using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models.Visit
{
    public class VisitProduct
    {
        public int VisitId { get; set; }

        public Visit Visit { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

    }
}
