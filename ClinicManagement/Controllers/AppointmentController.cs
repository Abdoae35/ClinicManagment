using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly IClinicService _clinicService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Helpers.AuditService _audit;

        public AppointmentController(IAppointmentService appointmentService, IDoctorService doctorService,
            IPatientService patientService, IClinicService clinicService, UserManager<ApplicationUser> userManager, Helpers.AuditService audit)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
            _clinicService = clinicService;
            _userManager = userManager;
            _audit = audit;
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            var events = appointments.Select(a => new AppointmentCalendarEvent
            {
                Id = a.AppointmentId,
                Title = $"{a.Patient.User.FullName} - {a.Doctor.User.FullName}",
                Start = a.AppointmentDate.Date.Add(a.AppointmentTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                End = a.AppointmentDate.Date.Add(a.AppointmentTime).AddMinutes(a.DurationMinutes).ToString("yyyy-MM-ddTHH:mm:ss"),
                Color = a.Status switch
                {
                    AppointmentStatus.Scheduled => "#0077B6",
                    AppointmentStatus.Confirmed => "#00B4D8",
                    AppointmentStatus.Completed => "#2DC653",
                    AppointmentStatus.Cancelled => "#E63946",
                    AppointmentStatus.NoShow => "#FFC300",
                    _ => "#0077B6"
                },
                Url = Url.Action("Details", new { id = a.AppointmentId })!
            }).ToList();
            ViewBag.Events = System.Text.Json.JsonSerializer.Serialize(events);
            return View(appointments);
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> List(DateTime? from, DateTime? to, int? doctorId, int? status)
        {
            var start = from ?? DateTime.Today.AddMonths(-1);
            var end = to ?? DateTime.Today.AddMonths(1);
            AppointmentStatus? st = status.HasValue ? (AppointmentStatus)status.Value : null;
            var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(start, end, doctorId, st);
            ViewBag.Doctors = await _doctorService.GetAllDoctorsAsync();
            ViewBag.From = start; ViewBag.To = end; ViewBag.DoctorId = doctorId; ViewBag.Status = status;
            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appt = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();
            return View(appt);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new AppointmentCreateViewModel
            {
                Specializations = await _doctorService.GetAllSpecializationsAsync(),
                Patients = await _patientService.GetAllPatientsAsync(),
                Clinics = await _clinicService.GetAllClinicsAsync(),
                AppointmentDate = DateTime.Today.AddDays(1)
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Specializations = await _doctorService.GetAllSpecializationsAsync();
                model.Patients = await _patientService.GetAllPatientsAsync();
                model.Clinics = await _clinicService.GetAllClinicsAsync();
                return View(model);
            }
            var appointment = new Appointment
            {
                PatientId = model.PatientId, DoctorId = model.DoctorId, ClinicId = model.ClinicId,
                AppointmentDate = model.AppointmentDate, AppointmentTime = model.AppointmentTime,
                DurationMinutes = model.DurationMinutes, ReasonForVisit = model.ReasonForVisit, Notes = model.Notes
            };
            var (success, message) = await _appointmentService.CreateAppointmentAsync(appointment);
            if (!success)
            {
                ModelState.AddModelError("", message);
                model.Specializations = await _doctorService.GetAllSpecializationsAsync();
                model.Patients = await _patientService.GetAllPatientsAsync();
                model.Clinics = await _clinicService.GetAllClinicsAsync();
                return View(model);
            }
            await _audit.LogAsync("Create", "Appointment", appointment.AppointmentId.ToString());
            TempData["Success"] = message;
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appt = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();
            var vm = new AppointmentEditViewModel
            {
                AppointmentId = appt.AppointmentId, PatientId = appt.PatientId, DoctorId = appt.DoctorId,
                ClinicId = appt.ClinicId, AppointmentDate = appt.AppointmentDate, AppointmentTime = appt.AppointmentTime,
                DurationMinutes = appt.DurationMinutes, Status = appt.Status, ReasonForVisit = appt.ReasonForVisit,
                Notes = appt.Notes, Clinics = await _clinicService.GetAllClinicsAsync()
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Clinics = await _clinicService.GetAllClinicsAsync();
                return View(model);
            }
            var appt = await _appointmentService.GetAppointmentByIdAsync(model.AppointmentId);
            if (appt == null) return NotFound();

            appt.ClinicId = model.ClinicId; appt.AppointmentDate = model.AppointmentDate;
            appt.AppointmentTime = model.AppointmentTime; appt.DurationMinutes = model.DurationMinutes;
            appt.Status = model.Status; appt.ReasonForVisit = model.ReasonForVisit; appt.Notes = model.Notes;

            await _appointmentService.UpdateAppointmentAsync(appt);
            TempData["Success"] = "Appointment updated.";
            return RedirectToAction("Details", new { id = model.AppointmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
        {
            var (success, message) = await _appointmentService.UpdateStatusAsync(id, status);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var (success, message) = await _appointmentService.CancelAppointmentAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("Details", new { id });
        }

        // API endpoints for AJAX
        [HttpGet]
        public async Task<IActionResult> GetDoctorsBySpecialization(int specializationId)
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specializationId);
            return Json(doctors.Select(d => new { d.DoctorId, Name = d.User.FullName, d.ConsultationFee }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, string date)
        {
            if (!DateTime.TryParse(date, out var dt)) return Json(new List<string>());
            var slots = await _doctorService.GetAvailableSlotsAsync(doctorId, dt);
            return Json(slots.Select(s => s.ToString(@"hh\:mm")));
        }
    }
}
