﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models.Visit
{
    public class VisitRequestBody
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Column("start_date")]
        public DateTimeOffset Start { get; set; }

        [Column("end_date")]
        public DateTimeOffset End { get; set; }

        [Column("is_all_day")]
        public bool AllDay { get; set; }

        public List<Product> ProductIds { get; set; }
    }
}