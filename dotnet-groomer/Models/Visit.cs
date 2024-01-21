using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_groomer.Models
{
    public class Visit
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Column("start_date")]
        public string StartDate { get; set; }

        [Column("end_date")]
        public string EndDate { get; set; }
        
        public bool IsAllDay { get; set; }
    }
}
