using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models.Visit
{
    public class Visit
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Column("start_date")]
        public DateTime Start { get; set; }

        [Column("end_date")]
        public DateTime End { get; set; }

        [Column("is_all_day")]
        public bool AllDay { get; set; }

        public List<VisitProduct> VisitProducts { get; set; }
    }
}
