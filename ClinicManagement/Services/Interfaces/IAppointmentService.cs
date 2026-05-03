using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Services.Interfaces
{
    /// <summary>Service for managing appointment operations.</summary>
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end, int? doctorId = null, AppointmentStatus? status = null);
        /// <summary>Creates a new appointment with double-booking validation.</summary>
        Task<(bool Success, string Message)> CreateAppointmentAsync(Appointment appointment);
        /// <summary>Updates an appointment.</summary>
        Task<(bool Success, string Message)> UpdateAppointmentAsync(Appointment appointment);
        /// <summary>Updates appointment status with business rule enforcement.</summary>
        Task<(bool Success, string Message)> UpdateStatusAsync(int appointmentId, AppointmentStatus status);
        /// <summary>Cancels an appointment (blocked if less than 2 hours before).</summary>
        Task<(bool Success, string Message)> CancelAppointmentAsync(int appointmentId);
        Task<int> GetTodayAppointmentCountAsync();
    }
}
