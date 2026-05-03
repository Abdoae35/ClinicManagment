using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models.Entities
{
    public class Specialization
    {
        [Key]
        public int SpecializationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Icon Class")]
        public string? IconClass { get; set; }

        // Navigation properties
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public virtual ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();
    }
}
