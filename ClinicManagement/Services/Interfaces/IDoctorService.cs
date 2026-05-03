using ClinicManagement.Models.Entities;

namespace ClinicManagement.Services.Interfaces
{
    /// <summary>Service for managing doctor operations.</summary>
    public interface IDoctorService
    {
        /// <summary>Gets all active doctors.</summary>
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
        /// <summary>Gets a doctor by ID with related data.</summary>
        Task<Doctor?> GetDoctorByIdAsync(int id);
        /// <summary>Gets a doctor by their user ID.</summary>
        Task<Doctor?> GetDoctorByUserIdAsync(string userId);
        /// <summary>Gets doctors filtered by specialization.</summary>
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId);
        /// <summary>Creates a new doctor profile.</summary>
        Task CreateDoctorAsync(Doctor doctor);
        /// <summary>Updates an existing doctor profile.</summary>
        Task UpdateDoctorAsync(Doctor doctor);
        /// <summary>Soft-deletes a doctor by setting IsAvailable to false.</summary>
        Task SoftDeleteDoctorAsync(int id);
        /// <summary>Gets all specializations.</summary>
        Task<IEnumerable<Specialization>> GetAllSpecializationsAsync();
        /// <summary>Gets doctor schedules.</summary>
        Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(int doctorId);
        /// <summary>Saves or updates a doctor's schedule.</summary>
        Task SaveScheduleAsync(int doctorId, List<DoctorSchedule> schedules);
        /// <summary>Gets available time slots for a doctor on a specific date.</summary>
        Task<List<TimeSpan>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    }
}
