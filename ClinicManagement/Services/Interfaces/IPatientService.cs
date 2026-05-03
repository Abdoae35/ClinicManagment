using ClinicManagement.Models.Entities;

namespace ClinicManagement.Services.Interfaces
{
    /// <summary>Service for managing patient operations.</summary>
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient?> GetPatientByUserIdAsync(string userId);
        Task CreatePatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
    }
}
