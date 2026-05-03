using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        public PatientService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
            => await _context.Patients.Include(p => p.User).Where(p => p.User.IsActive).ToListAsync();

        public async Task<Patient?> GetPatientByIdAsync(int id)
            => await _context.Patients.Include(p => p.User).Include(p => p.Appointments).ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .Include(p => p.MedicalRecords).Include(p => p.Invoices).FirstOrDefaultAsync(p => p.PatientId == id);

        public async Task<Patient?> GetPatientByUserIdAsync(string userId)
            => await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);

        public async Task CreatePatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _context.Patients.Include(p => p.User)
                .Where(p => p.User.FullName.ToLower().Contains(term) || p.User.PhoneNumber!.Contains(term) || (p.InsuranceNumber != null && p.InsuranceNumber.Contains(term)))
                .ToListAsync();
        }
    }
}
