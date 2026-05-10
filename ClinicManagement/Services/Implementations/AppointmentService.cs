using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IInvoiceService _invoiceService;

        public AppointmentService(ApplicationDbContext context, INotificationService notificationService, IInvoiceService invoiceService)
        {
            _context = context;
            _notificationService = notificationService;
            _invoiceService = invoiceService;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
            => await _context.Appointments.Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Clinic).OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime).ToListAsync();

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
            => await _context.Appointments.Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialization)
                .Include(a => a.Clinic).Include(a => a.MedicalRecord).Include(a => a.Invoice)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
            => await _context.Appointments.Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Clinic).Where(a => a.DoctorId == doctorId).OrderByDescending(a => a.AppointmentDate).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
            => await _context.Appointments.Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Clinic).Where(a => a.PatientId == patientId).OrderByDescending(a => a.AppointmentDate).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync()
            => await _context.Appointments.Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate.Date == DateTime.Today).OrderBy(a => a.AppointmentTime).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end, int? doctorId = null, AppointmentStatus? status = null)
        {
            var query = _context.Appointments.Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate.Date >= start.Date && a.AppointmentDate.Date <= end.Date);
            if (doctorId.HasValue) query = query.Where(a => a.DoctorId == doctorId.Value);
            if (status.HasValue) query = query.Where(a => a.Status == status.Value);
            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }

        public async Task<(bool Success, string Message)> CreateAppointmentAsync(Appointment appointment)
        {
            // Check double-booking
            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                a.AppointmentTime == appointment.AppointmentTime &&
                a.Status != AppointmentStatus.Cancelled);
            if (conflict) return (false, "This time slot is already booked for this doctor.");

            // Validate within schedule
            var schedule = await _context.DoctorSchedules.FirstOrDefaultAsync(s =>
                s.DoctorId == appointment.DoctorId && s.DayOfWeek == appointment.AppointmentDate.DayOfWeek && s.IsActive);
            if (schedule == null) return (false, "Doctor is not available on this day.");
            if (appointment.AppointmentTime < schedule.StartTime || appointment.AppointmentTime >= schedule.EndTime)
                return (false, "Selected time is outside doctor's working hours.");

            appointment.CreatedAt = DateTime.UtcNow;
            appointment.Status = AppointmentStatus.Scheduled;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Notify
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.DoctorId == appointment.DoctorId);
            if (doctor != null)
                await _notificationService.CreateNotificationAsync(doctor.UserId, "New Appointment", $"You have a new appointment on {appointment.AppointmentDate:MMM dd, yyyy}", NotificationType.Appointment);

            return (true, "Appointment created successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateAppointmentAsync(Appointment appointment)
        {
            appointment.UpdatedAt = DateTime.UtcNow;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return (true, "Appointment updated successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateStatusAsync(int appointmentId, AppointmentStatus status)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) return (false, "Appointment not found.");

            appt.Status = status;
            appt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (status == AppointmentStatus.Completed)
                await _invoiceService.CreateInvoiceAsync(appointmentId);

            return (true, $"Status updated to {status}.");
        }

        public async Task<(bool Success, string Message)> CancelAppointmentAsync(int appointmentId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null) return (false, "Appointment not found.");

            var appointmentDateTime = appt.AppointmentDate.Date + appt.AppointmentTime;
            if (appointmentDateTime - DateTime.Now < TimeSpan.FromHours(2))
                return (false, "Cannot cancel an appointment less than 2 hours before its scheduled time.");

            appt.Status = AppointmentStatus.Cancelled;
            appt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, "Appointment cancelled successfully.");
        }

        public async Task<int> GetTodayAppointmentCountAsync()
            => await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == DateTime.Today);
    }
}
