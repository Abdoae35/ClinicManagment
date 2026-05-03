using ClinicManagement.Models.Entities;

namespace ClinicManagement.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecord?> GetRecordByIdAsync(int id);
        Task<IEnumerable<MedicalRecord>> GetRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetRecordsByDoctorAsync(int doctorId);
        /// <summary>Creates a medical record (appointment must be Completed).</summary>
        Task<(bool Success, string Message)> CreateRecordAsync(MedicalRecord record);
        Task<bool> HasRecordForAppointmentAsync(int appointmentId);
    }
}
