using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Helpers;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class ClinicController : Controller
    {
        private readonly IClinicService _clinicService;
        private readonly IDoctorService _doctorService;
        private readonly AuditService _audit;

        public ClinicController(IClinicService clinicService, IDoctorService doctorService, AuditService audit)
        { _clinicService = clinicService; _doctorService = doctorService; _audit = audit; }

        public async Task<IActionResult> Index()
        {
            var clinics = await _clinicService.GetAllClinicsAsync();
            return View(clinics);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ClinicCreateViewModel { Specializations = await _doctorService.GetAllSpecializationsAsync() };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClinicCreateViewModel model)
        {
            if (!ModelState.IsValid) { model.Specializations = await _doctorService.GetAllSpecializationsAsync(); return View(model); }
            var clinic = new Clinic
            {
                Name = model.Name, Description = model.Description, Floor = model.Floor,
                RoomNumber = model.RoomNumber, Phone = model.Phone, SpecializationId = model.SpecializationId, IsActive = true
            };
            await _clinicService.CreateClinicAsync(clinic);
            await _audit.LogAsync("Create", "Clinic", clinic.ClinicId.ToString());
            TempData["Success"] = "Clinic created.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var clinic = await _clinicService.GetClinicByIdAsync(id);
            if (clinic == null) return NotFound();
            var vm = new ClinicEditViewModel
            {
                ClinicId = clinic.ClinicId, Name = clinic.Name, Description = clinic.Description,
                Floor = clinic.Floor, RoomNumber = clinic.RoomNumber, Phone = clinic.Phone,
                SpecializationId = clinic.SpecializationId, IsActive = clinic.IsActive,
                Specializations = await _doctorService.GetAllSpecializationsAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClinicEditViewModel model)
        {
            if (!ModelState.IsValid) { model.Specializations = await _doctorService.GetAllSpecializationsAsync(); return View(model); }
            var clinic = await _clinicService.GetClinicByIdAsync(model.ClinicId);
            if (clinic == null) return NotFound();
            clinic.Name = model.Name; clinic.Description = model.Description; clinic.Floor = model.Floor;
            clinic.RoomNumber = model.RoomNumber; clinic.Phone = model.Phone;
            clinic.SpecializationId = model.SpecializationId; clinic.IsActive = model.IsActive;
            await _clinicService.UpdateClinicAsync(clinic);
            TempData["Success"] = "Clinic updated.";
            return RedirectToAction("Index");
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _clinicService.DeleteClinicAsync(id);
            TempData["Success"] = "Clinic deactivated.";
            return RedirectToAction("Index");
        }
    }
}
