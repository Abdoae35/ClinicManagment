using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class ClinicService : IClinicService
    {
        private readonly ApplicationDbContext _context;
        public ClinicService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Clinic>> GetAllClinicsAsync()
            => await _context.Clinics.Include(c => c.Specialization).ToListAsync();

        public async Task<Clinic?> GetClinicByIdAsync(int id)
            => await _context.Clinics.Include(c => c.Specialization).FirstOrDefaultAsync(c => c.ClinicId == id);

        public async Task CreateClinicAsync(Clinic clinic)
        { _context.Clinics.Add(clinic); await _context.SaveChangesAsync(); }

        public async Task UpdateClinicAsync(Clinic clinic)
        { _context.Clinics.Update(clinic); await _context.SaveChangesAsync(); }

        public async Task DeleteClinicAsync(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic != null) { clinic.IsActive = false; await _context.SaveChangesAsync(); }
        }
    }
}
