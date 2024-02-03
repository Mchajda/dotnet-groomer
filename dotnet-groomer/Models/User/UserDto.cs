using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_groomer.Models.Visit;

namespace dotnet_groomer.Models.User
{
    public class UserDto
    {
        public int? Id { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
