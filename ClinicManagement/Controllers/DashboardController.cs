using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly IInvoiceService _invoiceService;
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, IAppointmentService appointmentService,
            IInvoiceService invoiceService, IDoctorService doctorService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appointmentService = appointmentService;
            _invoiceService = invoiceService;
            _doctorService = doctorService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var last7 = Enumerable.Range(-6, 7).Select(i => today.AddDays(i)).ToList();

            var apptsByDay = new Dictionary<string, int>();
            foreach (var d in last7)
            {
                var count = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == d.Date);
                apptsByDay[d.ToString("MMM dd")] = count;
            }

            var apptsByStatus = await _context.Appointments.GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() }).ToListAsync();

            var patientsBySpec = await _context.Patients
                .Join(_context.Appointments, p => p.PatientId, a => a.PatientId, (p, a) => a)
                .Join(_context.Doctors, a => a.DoctorId, d => d.DoctorId, (a, d) => d)
                .Join(_context.Specializations, d => d.SpecializationId, s => s.SpecializationId, (d, s) => s.Name)
                .GroupBy(name => name).Select(g => new { Name = g.Key, Count = g.Count() }).ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TodayAppointments = await _appointmentService.GetTodayAppointmentCountAsync(),
                AvailableDoctors = await _context.Doctors.CountAsync(d => d.IsAvailable),
                PendingInvoices = await _invoiceService.GetPendingInvoiceCountAsync(),
                TotalRevenue = await _invoiceService.GetTotalRevenueAsync(),
                TodayAppointmentsList = await _appointmentService.GetTodayAppointmentsAsync(),
                AppointmentsPerDay = apptsByDay,
                AppointmentsByStatus = apptsByStatus.ToDictionary(x => x.Status, x => x.Count),
                PatientsBySpecialization = patientsBySpec.ToDictionary(x => x.Name, x => x.Count)
            };
            return View(vm);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Doctor()
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(user!.Id);
            if (doctor == null) return NotFound();

            var todayAppts = (await _appointmentService.GetAppointmentsByDoctorAsync(doctor.DoctorId))
                .Where(a => a.AppointmentDate.Date == DateTime.Today).ToList();

            var completedWithoutRecord = (await _appointmentService.GetAppointmentsByDoctorAsync(doctor.DoctorId))
                .Where(a => a.Status == AppointmentStatus.Completed && a.MedicalRecord == null).Count();

            var schedules = await _doctorService.GetDoctorSchedulesAsync(doctor.DoctorId);

            var vm = new DoctorDashboardViewModel
            {
                TodayAppointments = todayAppts.Count,
                PendingRecords = completedWithoutRecord,
                TodayAppointmentsList = todayAppts,
                WeekSchedule = schedules
            };
            return View(vm);
        }
    }
}
