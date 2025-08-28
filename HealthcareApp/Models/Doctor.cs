using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareApp.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; }

        [MaxLength(500)]
        public string? Availability { get; set; } // e.g., Mon-Fri 9am-5pm

        [MaxLength(1000)]
        public string? ScheduleNotes { get; set; }
    }
}
