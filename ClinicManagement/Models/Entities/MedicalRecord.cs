using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Models.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [Required]
        [Display(Name = "Appointment")]
        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Diagnosis { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Prescription { get; set; }

        [StringLength(2000)]
        [Display(Name = "Treatment Plan")]
        public string? TreatmentPlan { get; set; }

        [StringLength(1000)]
        [Display(Name = "Lab Results")]
        public string? LabResults { get; set; }

        [StringLength(500)]
        [Display(Name = "Attachment Path")]
        public string? AttachmentPath { get; set; }

        [Display(Name = "Follow-Up Date")]
        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
