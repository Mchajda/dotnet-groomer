using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_groomer.Models.Visit;

namespace dotnet_groomer.Models.User
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Dog { get; set; }

        [Column("visit_count")]
        public int VisitCount { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        public List<Visit.Visit> Visits { get; set; }
    }
}
