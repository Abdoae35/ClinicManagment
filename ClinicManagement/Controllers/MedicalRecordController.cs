using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize]
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordService _recordService;
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorService _doctorService;

        public MedicalRecordController(IMedicalRecordService recordService, IAppointmentService appointmentService,
            UserManager<ApplicationUser> userManager, IDoctorService doctorService)
        {
            _recordService = recordService;
            _appointmentService = appointmentService;
            _userManager = userManager;
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var appt = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();
            if (await _recordService.HasRecordForAppointmentAsync(id))
            { TempData["Error"] = "A medical record already exists."; return RedirectToAction("Details", "Appointment", new { id = id }); }

            var vm = new MedicalRecordCreateViewModel
            {
                AppointmentId = id, PatientId = appt.PatientId, DoctorId = appt.DoctorId,
                PatientName = appt.Patient.User.FullName, DoctorName = appt.Doctor.User.FullName,
                AppointmentDate = appt.AppointmentDate
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string? attachmentPath = null;
            if (model.Attachment != null && model.Attachment.Length > 0)
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "records");
                Directory.CreateDirectory(dir);
                var fn = $"{Guid.NewGuid()}_{model.Attachment.FileName}";
                using var stream = new FileStream(Path.Combine(dir, fn), FileMode.Create);
                await model.Attachment.CopyToAsync(stream);
                attachmentPath = $"/uploads/records/{fn}";
            }

            var record = new MedicalRecord
            {
                AppointmentId = model.AppointmentId, PatientId = model.PatientId, DoctorId = model.DoctorId,
                Diagnosis = model.Diagnosis, Prescription = model.Prescription, TreatmentPlan = model.TreatmentPlan,
                LabResults = model.LabResults, AttachmentPath = attachmentPath, FollowUpDate = model.FollowUpDate
            };

            // Add prescription items
            if (model.PrescriptionItems?.Any() == true)
            {
                foreach (var item in model.PrescriptionItems.Where(p => !string.IsNullOrEmpty(p.MedicineName)))
                {
                    record.PrescriptionItems.Add(new PrescriptionItem
                    {
                        MedicineName = item.MedicineName, Dosage = item.Dosage,
                        Frequency = item.Frequency, Duration = item.Duration, Instructions = item.Instructions
                    });
                }
            }

            var (success, message) = await _recordService.CreateRecordAsync(record);
            if (!success) { TempData["Error"] = message; return View(model); }
            TempData["Success"] = message;
            return RedirectToAction("Details", new { id = record.RecordId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var record = await _recordService.GetRecordByIdAsync(id);
            if (record == null) return NotFound();
            return View(record);
        }

        public async Task<IActionResult> Print(int id)
        {
            var record = await _recordService.GetRecordByIdAsync(id);
            if (record == null) return NotFound();
            return View(record);
        }
    }
}
