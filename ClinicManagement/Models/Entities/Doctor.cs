using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Models.Entities
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [Display(Name = "Specialization")]
        public int SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public virtual Specialization Specialization { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Display(Name = "Years of Experience")]
        [Range(0, 60)]
        public int YearsOfExperience { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        [Display(Name = "Consultation Fee")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000)]
        public decimal ConsultationFee { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Working Days")]
        public string? WorkingDays { get; set; }

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    }
}
