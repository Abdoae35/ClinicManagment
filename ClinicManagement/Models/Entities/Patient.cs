using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Models.Entities
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [Display(Name = "Blood Type")]
        public BloodType? BloodType { get; set; }

        [StringLength(100)]
        [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        [Display(Name = "Emergency Contact Phone")]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(50)]
        [Display(Name = "Insurance Number")]
        public string? InsuranceNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Insurance Provider")]
        public string? InsuranceProvider { get; set; }

        [StringLength(2000)]
        [Display(Name = "Medical History")]
        public string? MedicalHistory { get; set; }

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
