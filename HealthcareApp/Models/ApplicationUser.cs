using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthcareApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Role { get; set; } // "Admin", "Doctor", "Patient"
    }
}