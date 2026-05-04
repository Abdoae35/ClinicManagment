using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Helpers;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _audit;

        public PatientController(IPatientService patientService, UserManager<ApplicationUser> userManager, AuditService audit)
        {
            _patientService = patientService;
            _userManager = userManager;
            _audit = audit;
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Index(string? search)
        {
            var patients = string.IsNullOrEmpty(search)
                ? await _patientService.GetAllPatientsAsync()
                : await _patientService.SearchPatientsAsync(search);
            ViewBag.Search = search;
            return View(patients);
        }

        [Authorize(Roles = "Admin,Receptionist,Doctor")]
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser
            {
                UserName = model.Email, Email = model.Email, FullName = model.FullName,
                PhoneNumber = model.PhoneNumber, Gender = model.Gender, DateOfBirth = model.DateOfBirth,
                Address = model.Address, EmailConfirmed = true, IsActive = true, CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, "Patient@123456");
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, "Patient");

            var patient = new Patient
            {
                UserId = user.Id, BloodType = model.BloodType, EmergencyContactName = model.EmergencyContactName,
                EmergencyContactPhone = model.EmergencyContactPhone, InsuranceNumber = model.InsuranceNumber,
                InsuranceProvider = model.InsuranceProvider, MedicalHistory = model.MedicalHistory
            };
            await _patientService.CreatePatientAsync(patient);
            await _audit.LogAsync("Create", "Patient", patient.PatientId.ToString(), $"Created patient {model.FullName}");
            TempData["Success"] = "Patient created successfully.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound();
            var vm = new PatientEditViewModel
            {
                PatientId = patient.PatientId, UserId = patient.UserId, FullName = patient.User.FullName,
                Email = patient.User.Email!, PhoneNumber = patient.User.PhoneNumber, Gender = patient.User.Gender,
                DateOfBirth = patient.User.DateOfBirth, Address = patient.User.Address, BloodType = patient.BloodType,
                EmergencyContactName = patient.EmergencyContactName, EmergencyContactPhone = patient.EmergencyContactPhone,
                InsuranceNumber = patient.InsuranceNumber, InsuranceProvider = patient.InsuranceProvider,
                MedicalHistory = patient.MedicalHistory
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var patient = await _patientService.GetPatientByIdAsync(model.PatientId);
            if (patient == null) return NotFound();

            patient.User.FullName = model.FullName;
            patient.User.PhoneNumber = model.PhoneNumber;
            patient.User.Gender = model.Gender;
            patient.User.DateOfBirth = model.DateOfBirth;
            patient.User.Address = model.Address;
            patient.BloodType = model.BloodType;
            patient.EmergencyContactName = model.EmergencyContactName;
            patient.EmergencyContactPhone = model.EmergencyContactPhone;
            patient.InsuranceNumber = model.InsuranceNumber;
            patient.InsuranceProvider = model.InsuranceProvider;
            patient.MedicalHistory = model.MedicalHistory;

            await _patientService.UpdatePatientAsync(patient);
            await _audit.LogAsync("Update", "Patient", model.PatientId.ToString());
            TempData["Success"] = "Patient updated successfully.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _patientService.DeletePatientAsync(id);
            await _audit.LogAsync("SoftDelete", "Patient", id.ToString());
            TempData["Success"] = "Patient deleted successfully.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var patient = await _patientService.GetPatientByUserIdAsync(user.Id);
            if (patient == null) return NotFound();
            // Re-fetch with includes
            patient = await _patientService.GetPatientByIdAsync(patient.PatientId);
            return View(patient);
        }
    }
}
