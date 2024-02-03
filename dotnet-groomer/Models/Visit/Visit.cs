using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_groomer.Models.User;
using Newtonsoft.Json;

namespace dotnet_groomer.Models.Visit
{
    public class Visit
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Column("start_date")]
        public DateTimeOffset Start { get; set; }
        [Column("end_date")]
        public DateTimeOffset End { get; set; }
        [Column("is_all_day")]
        public bool AllDay { get; set; }
        public int Price { get; set; }
        [Column("cleared")]
        public bool PaymentCleared { get; set; } = false;
        public List<VisitProduct> VisitProducts { get; set; }
        [Column("customer_id")]
        public int? CustomerId { get; set; }
        public User.User? Customer { get; set; }
    }
}
