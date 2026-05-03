using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        public DoctorService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
            => await _context.Doctors.Include(d => d.User).Include(d => d.Specialization).Where(d => d.IsAvailable).ToListAsync();

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
            => await _context.Doctors.Include(d => d.User).Include(d => d.Specialization).Include(d => d.Schedules).FirstOrDefaultAsync(d => d.DoctorId == id);

        public async Task<Doctor?> GetDoctorByUserIdAsync(string userId)
            => await _context.Doctors.Include(d => d.User).Include(d => d.Specialization).FirstOrDefaultAsync(d => d.UserId == userId);

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId)
            => await _context.Doctors.Include(d => d.User).Include(d => d.Specialization).Where(d => d.SpecializationId == specializationId && d.IsAvailable).ToListAsync();

        public async Task CreateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null) { doctor.IsAvailable = false; await _context.SaveChangesAsync(); }
        }

        public async Task<IEnumerable<Specialization>> GetAllSpecializationsAsync()
            => await _context.Specializations.ToListAsync();

        public async Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(int doctorId)
            => await _context.DoctorSchedules.Where(s => s.DoctorId == doctorId).OrderBy(s => s.DayOfWeek).ToListAsync();

        public async Task SaveScheduleAsync(int doctorId, List<DoctorSchedule> schedules)
        {
            var existing = await _context.DoctorSchedules.Where(s => s.DoctorId == doctorId).ToListAsync();
            _context.DoctorSchedules.RemoveRange(existing);
            foreach (var s in schedules) { s.DoctorId = doctorId; _context.DoctorSchedules.Add(s); }
            await _context.SaveChangesAsync();
        }

        public async Task<List<TimeSpan>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;
            var schedule = await _context.DoctorSchedules.FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek && s.IsActive);
            if (schedule == null) return new List<TimeSpan>();

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.AppointmentTime).ToListAsync();

            var slots = new List<TimeSpan>();
            var current = schedule.StartTime;
            while (current < schedule.EndTime)
            {
                if (!bookedSlots.Contains(current)) slots.Add(current);
                current = current.Add(TimeSpan.FromMinutes(schedule.SlotDurationMinutes));
            }
            return slots;
        }
    }
}
