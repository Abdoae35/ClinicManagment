using System.ComponentModel.DataAnnotations;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Models.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
        [Required][DataType(DataType.Date)] public DateTime AppointmentDate { get; set; }
        [Required] public TimeSpan AppointmentTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        [StringLength(500)] public string? ReasonForVisit { get; set; }
        public string? Notes { get; set; }
        public int? SpecializationId { get; set; }

        // For dropdowns
        public IEnumerable<Specialization>? Specializations { get; set; }
        public IEnumerable<Doctor>? Doctors { get; set; }
        public IEnumerable<Patient>? Patients { get; set; }
        public IEnumerable<Clinic>? Clinics { get; set; }
        public List<TimeSpan>? AvailableSlots { get; set; }
    }

    public class AppointmentEditViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
        [Required][DataType(DataType.Date)] public DateTime AppointmentDate { get; set; }
        [Required] public TimeSpan AppointmentTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        public AppointmentStatus Status { get; set; }
        [StringLength(500)] public string? ReasonForVisit { get; set; }
        public string? Notes { get; set; }

        public IEnumerable<Clinic>? Clinics { get; set; }
    }

    public class AppointmentCalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
