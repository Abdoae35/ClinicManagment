using System.ComponentModel.DataAnnotations;
using ClinicManagement.Models.Entities;

namespace ClinicManagement.Models.ViewModels
{
    public class DoctorCreateViewModel
    {
        [Required] public string UserId { get; set; } = string.Empty;
        [Required] public int SpecializationId { get; set; }
        [Required][StringLength(50)] public string LicenseNumber { get; set; } = string.Empty;
        [Range(0, 60)] public int YearsOfExperience { get; set; }
        [StringLength(1000)] public string? Bio { get; set; }
        [Range(0, 10000)] public decimal ConsultationFee { get; set; }
        public string? WorkingDays { get; set; }

        // For form dropdowns
        [Required][StringLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Phone] public string? PhoneNumber { get; set; }
        public string? Password { get; set; } = "Doctor@123456";
        public IEnumerable<Specialization>? Specializations { get; set; }
    }

    public class DoctorEditViewModel
    {
        public int DoctorId { get; set; }
        public string UserId { get; set; } = string.Empty;
        [Required] public int SpecializationId { get; set; }
        [Required][StringLength(50)] public string LicenseNumber { get; set; } = string.Empty;
        [Range(0, 60)] public int YearsOfExperience { get; set; }
        [StringLength(1000)] public string? Bio { get; set; }
        [Range(0, 10000)] public decimal ConsultationFee { get; set; }
        public bool IsAvailable { get; set; }
        public string? WorkingDays { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IEnumerable<Specialization>? Specializations { get; set; }
    }

    public class DoctorScheduleViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public List<ScheduleEntry> Schedules { get; set; } = new();
    }

    public class ScheduleEntry
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
        public bool IsActive { get; set; }
    }
}
