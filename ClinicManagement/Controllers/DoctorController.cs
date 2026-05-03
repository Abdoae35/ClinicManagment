using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Helpers;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _audit;

        public DoctorController(IDoctorService doctorService, UserManager<ApplicationUser> userManager, AuditService audit)
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _audit = audit;
        }

        public async Task<IActionResult> Index(string? search, int? specializationId)
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            if (!string.IsNullOrEmpty(search))
                doctors = doctors.Where(d => d.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));
            if (specializationId.HasValue)
                doctors = doctors.Where(d => d.SpecializationId == specializationId.Value);

            ViewBag.Specializations = await _doctorService.GetAllSpecializationsAsync();
            ViewBag.Search = search;
            ViewBag.SpecializationId = specializationId;
            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new DoctorCreateViewModel { Specializations = await _doctorService.GetAllSpecializationsAsync() };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Specializations = await _doctorService.GetAllSpecializationsAsync();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email, Email = model.Email, FullName = model.FullName,
                PhoneNumber = model.PhoneNumber, EmailConfirmed = true, IsActive = true, CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, model.Password ?? "Doctor@123456");
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                model.Specializations = await _doctorService.GetAllSpecializationsAsync();
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctor = new Doctor
            {
                UserId = user.Id, SpecializationId = model.SpecializationId, LicenseNumber = model.LicenseNumber,
                YearsOfExperience = model.YearsOfExperience, Bio = model.Bio, ConsultationFee = model.ConsultationFee,
                IsAvailable = true, WorkingDays = model.WorkingDays
            };
            await _doctorService.CreateDoctorAsync(doctor);
            await _audit.LogAsync("Create", "Doctor", doctor.DoctorId.ToString(), $"Created doctor {model.FullName}");
            TempData["Success"] = "Doctor created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            var vm = new DoctorEditViewModel
            {
                DoctorId = doctor.DoctorId, UserId = doctor.UserId, SpecializationId = doctor.SpecializationId,
                LicenseNumber = doctor.LicenseNumber, YearsOfExperience = doctor.YearsOfExperience, Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee, IsAvailable = doctor.IsAvailable, WorkingDays = doctor.WorkingDays,
                FullName = doctor.User.FullName, Email = doctor.User.Email!, PhoneNumber = doctor.User.PhoneNumber,
                Specializations = await _doctorService.GetAllSpecializationsAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Specializations = await _doctorService.GetAllSpecializationsAsync();
                return View(model);
            }
            var doctor = await _doctorService.GetDoctorByIdAsync(model.DoctorId);
            if (doctor == null) return NotFound();

            doctor.SpecializationId = model.SpecializationId;
            doctor.LicenseNumber = model.LicenseNumber;
            doctor.YearsOfExperience = model.YearsOfExperience;
            doctor.Bio = model.Bio;
            doctor.ConsultationFee = model.ConsultationFee;
            doctor.IsAvailable = model.IsAvailable;
            doctor.WorkingDays = model.WorkingDays;
            doctor.User.FullName = model.FullName;
            doctor.User.PhoneNumber = model.PhoneNumber;

            await _doctorService.UpdateDoctorAsync(doctor);
            await _audit.LogAsync("Update", "Doctor", model.DoctorId.ToString(), $"Updated doctor {model.FullName}");
            TempData["Success"] = "Doctor updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _doctorService.SoftDeleteDoctorAsync(id);
            await _audit.LogAsync("SoftDelete", "Doctor", id.ToString());
            TempData["Success"] = "Doctor deactivated successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Schedule(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            var schedules = await _doctorService.GetDoctorSchedulesAsync(id);
            var vm = new DoctorScheduleViewModel
            {
                DoctorId = id, DoctorName = doctor.User.FullName,
                Schedules = schedules.Select(s => new ScheduleEntry
                {
                    DayOfWeek = s.DayOfWeek, StartTime = s.StartTime, EndTime = s.EndTime,
                    SlotDurationMinutes = s.SlotDurationMinutes, IsActive = s.IsActive
                }).ToList()
            };
            // Fill in missing days
            foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
            {
                if (!vm.Schedules.Any(s => s.DayOfWeek == day))
                    vm.Schedules.Add(new ScheduleEntry { DayOfWeek = day, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsActive = false });
            }
            vm.Schedules = vm.Schedules.OrderBy(s => s.DayOfWeek).ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(DoctorScheduleViewModel model)
        {
            var schedules = model.Schedules.Where(s => s.IsActive).Select(s => new DoctorSchedule
            {
                DayOfWeek = s.DayOfWeek, StartTime = s.StartTime, EndTime = s.EndTime,
                SlotDurationMinutes = s.SlotDurationMinutes, IsActive = true
            }).ToList();
            await _doctorService.SaveScheduleAsync(model.DoctorId, schedules);
            TempData["Success"] = "Schedule saved successfully.";
            return RedirectToAction("Schedule", new { id = model.DoctorId });
        }
    }
}
