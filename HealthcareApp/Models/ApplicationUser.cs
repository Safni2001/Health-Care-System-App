using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FullName { get; set; }

    [Required]
    public string Role { get; set; } // "Admin", "Doctor", "Patient"
}