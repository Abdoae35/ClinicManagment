using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Models.ViewModels;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IDoctorService _doctorService;

        public ReportController(IReportService reportService, IDoctorService doctorService)
        { _reportService = reportService; _doctorService = doctorService; }

        public async Task<IActionResult> Index()
        {
            ViewBag.Doctors = await _doctorService.GetAllDoctorsAsync();
            return View();
        }

        public async Task<IActionResult> Appointments(DateTime? from, DateTime? to, int? doctorId, int? status)
        {
            var vm = new ReportFilterViewModel
            {
                FromDate = from, ToDate = to, DoctorId = doctorId, Status = status,
                Doctors = await _doctorService.GetAllDoctorsAsync(),
                ReportData = await _reportService.GetAppointmentReportAsync(from, to, doctorId, status)
            };
            return View(vm);
        }

        public async Task<IActionResult> Revenue(DateTime? from, DateTime? to, int? doctorId)
        {
            var vm = new ReportFilterViewModel
            {
                FromDate = from, ToDate = to, DoctorId = doctorId,
                Doctors = await _doctorService.GetAllDoctorsAsync(),
                ReportData = await _reportService.GetRevenueReportAsync(from, to, doctorId)
            };
            return View(vm);
        }

        public async Task<IActionResult> ExportAppointments(DateTime? from, DateTime? to, int? doctorId, int? status)
        {
            var bytes = await _reportService.ExportAppointmentReportToExcelAsync(from, to, doctorId, status);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AppointmentsReport.xlsx");
        }

        public async Task<IActionResult> ExportRevenue(DateTime? from, DateTime? to, int? doctorId)
        {
            var bytes = await _reportService.ExportRevenueReportToExcelAsync(from, to, doctorId);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RevenueReport.xlsx");
        }
    }
}
