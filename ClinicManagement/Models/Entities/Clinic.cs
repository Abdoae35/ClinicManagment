using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Models.Entities
{
    public class Clinic
    {
        [Key]
        public int ClinicId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(10)]
        public string? Floor { get; set; }

        [StringLength(20)]
        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "Specialization")]
        public int? SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public virtual Specialization? Specialization { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
