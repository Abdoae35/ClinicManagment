using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly ApplicationDbContext _context;
        public MedicalRecordService(ApplicationDbContext context) => _context = context;

        public async Task<MedicalRecord?> GetRecordByIdAsync(int id)
            => await _context.MedicalRecords.Include(r => r.Patient).ThenInclude(p => p.User)
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Include(r => r.Appointment).Include(r => r.PrescriptionItems)
                .FirstOrDefaultAsync(r => r.RecordId == id);

        public async Task<IEnumerable<MedicalRecord>> GetRecordsByPatientAsync(int patientId)
            => await _context.MedicalRecords.Include(r => r.Doctor).ThenInclude(d => d.User)
                .Include(r => r.Appointment).Where(r => r.PatientId == patientId).OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<IEnumerable<MedicalRecord>> GetRecordsByDoctorAsync(int doctorId)
            => await _context.MedicalRecords.Include(r => r.Patient).ThenInclude(p => p.User)
                .Include(r => r.Appointment).Where(r => r.DoctorId == doctorId).OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<(bool Success, string Message)> CreateRecordAsync(MedicalRecord record)
        {
            var appointment = await _context.Appointments.FindAsync(record.AppointmentId);
            if (appointment == null) return (false, "Appointment not found.");
            if (appointment.Status != AppointmentStatus.Completed) return (false, "Medical record can only be created for completed appointments.");
            if (await HasRecordForAppointmentAsync(record.AppointmentId)) return (false, "A medical record already exists for this appointment.");

            record.CreatedAt = DateTime.UtcNow;
            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();
            return (true, "Medical record created successfully.");
        }

        public async Task<bool> HasRecordForAppointmentAsync(int appointmentId)
            => await _context.MedicalRecords.AnyAsync(r => r.AppointmentId == appointmentId);
    }
}
