using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models
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
    }
}
