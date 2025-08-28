using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; } = string.Empty; // FK to AspNetUsers
        public ApplicationUser? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Payment { get; set; }

        [MaxLength(200)]
        public string? Status { get; set; } // Scheduled, Completed, Cancelled
    }
}
