using ClinicManagement.Models.Entities;

namespace ClinicManagement.Services.Interfaces
{
    public interface IClinicService
    {
        Task<IEnumerable<Clinic>> GetAllClinicsAsync();
        Task<Clinic?> GetClinicByIdAsync(int id);
        Task CreateClinicAsync(Clinic clinic);
        Task UpdateClinicAsync(Clinic clinic);
        Task DeleteClinicAsync(int id);
    }
}
