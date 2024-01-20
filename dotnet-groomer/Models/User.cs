using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models
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
    }
}
