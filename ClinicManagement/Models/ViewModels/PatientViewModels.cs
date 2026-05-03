using System.ComponentModel.DataAnnotations;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Models.ViewModels
{
    public class PatientCreateViewModel
    {
        [Required][StringLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Phone] public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        [DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public BloodType? BloodType { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? InsuranceNumber { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? MedicalHistory { get; set; }
    }

    public class PatientEditViewModel
    {
        public int PatientId { get; set; }
        public string UserId { get; set; } = string.Empty;
        [Required][StringLength(100)] public string FullName { get; set; } = string.Empty;
        [EmailAddress] public string Email { get; set; } = string.Empty;
        [Phone] public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        [DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public BloodType? BloodType { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? InsuranceNumber { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? MedicalHistory { get; set; }
    }
}
