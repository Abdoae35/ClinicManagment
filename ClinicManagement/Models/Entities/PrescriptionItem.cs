using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Models.Entities
{
    public class PrescriptionItem
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        [Display(Name = "Medical Record")]
        public int RecordId { get; set; }

        [ForeignKey("RecordId")]
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;

        [Required]
        [StringLength(200)]
        [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Frequency { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Duration { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }
    }
}
