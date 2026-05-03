using System.ComponentModel.DataAnnotations;
using ClinicManagement.Models.Entities;

namespace ClinicManagement.Models.ViewModels
{
    public class MedicalRecordCreateViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        [Required][StringLength(2000)] public string Diagnosis { get; set; } = string.Empty;
        public string? Prescription { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? LabResults { get; set; }
        [DataType(DataType.Date)] public DateTime? FollowUpDate { get; set; }
        public IFormFile? Attachment { get; set; }
        public List<PrescriptionItemViewModel> PrescriptionItems { get; set; } = new();

        // Display
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }

    public class PrescriptionItemViewModel
    {
        [Required] public string MedicineName { get; set; } = string.Empty;
        [Required] public string Dosage { get; set; } = string.Empty;
        [Required] public string Frequency { get; set; } = string.Empty;
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
    }

    public class ClinicCreateViewModel
    {
        [Required][StringLength(100)] public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Floor { get; set; }
        public string? RoomNumber { get; set; }
        public string? Phone { get; set; }
        public int? SpecializationId { get; set; }
        public IEnumerable<Specialization>? Specializations { get; set; }
    }

    public class ClinicEditViewModel : ClinicCreateViewModel
    {
        public int ClinicId { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoginViewModel
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required][StringLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][DataType(DataType.Password)][MinLength(6)] public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)][Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        [Phone] public string? PhoneNumber { get; set; }
    }

    public class ProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        [Required] public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        public IFormFile? Avatar { get; set; }
        [DataType(DataType.Password)] public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)] public string? NewPassword { get; set; }
        [DataType(DataType.Password)][Compare("NewPassword")] public string? ConfirmNewPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required] public string Token { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)][Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ReportFilterViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? DoctorId { get; set; }
        public int? Status { get; set; }
        public IEnumerable<Doctor>? Doctors { get; set; }
        public object? ReportData { get; set; }
    }
}
