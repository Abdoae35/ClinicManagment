using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ClinicManagement.Data;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        public ReportService(ApplicationDbContext context) => _context = context;

        public async Task<object> GetAppointmentReportAsync(DateTime? from, DateTime? to, int? doctorId, int? status)
        {
            var query = _context.Appointments.Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User).AsQueryable();
            if (from.HasValue) query = query.Where(a => a.AppointmentDate >= from.Value);
            if (to.HasValue) query = query.Where(a => a.AppointmentDate <= to.Value);
            if (doctorId.HasValue) query = query.Where(a => a.DoctorId == doctorId.Value);
            if (status.HasValue) query = query.Where(a => (int)a.Status == status.Value);
            var data = await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
            return data;
        }

        public async Task<object> GetRevenueReportAsync(DateTime? from, DateTime? to, int? doctorId)
        {
            var query = _context.Invoices.Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Appointment).ThenInclude(a => a.Doctor).ThenInclude(d => d.User).AsQueryable();
            if (from.HasValue) query = query.Where(i => i.InvoiceDate >= from.Value);
            if (to.HasValue) query = query.Where(i => i.InvoiceDate <= to.Value);
            if (doctorId.HasValue) query = query.Where(i => i.Appointment.DoctorId == doctorId.Value);
            var data = await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();
            return data;
        }

        public async Task<byte[]> ExportAppointmentReportToExcelAsync(DateTime? from, DateTime? to, int? doctorId, int? status)
        {
            var appointments = (IEnumerable<Models.Entities.Appointment>)await GetAppointmentReportAsync(from, to, doctorId, status);
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Appointments");
            ws.Cells[1, 1].Value = "Date"; ws.Cells[1, 2].Value = "Time"; ws.Cells[1, 3].Value = "Patient";
            ws.Cells[1, 4].Value = "Doctor"; ws.Cells[1, 5].Value = "Status"; ws.Cells[1, 6].Value = "Reason";
            int row = 2;
            foreach (var a in appointments)
            {
                ws.Cells[row, 1].Value = a.AppointmentDate.ToString("yyyy-MM-dd");
                ws.Cells[row, 2].Value = a.AppointmentTime.ToString(@"hh\:mm");
                ws.Cells[row, 3].Value = a.Patient?.User?.FullName;
                ws.Cells[row, 4].Value = a.Doctor?.User?.FullName;
                ws.Cells[row, 5].Value = a.Status.ToString();
                ws.Cells[row, 6].Value = a.ReasonForVisit;
                row++;
            }
            ws.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportRevenueReportToExcelAsync(DateTime? from, DateTime? to, int? doctorId)
        {
            var invoices = (IEnumerable<Models.Entities.Invoice>)await GetRevenueReportAsync(from, to, doctorId);
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Revenue");
            ws.Cells[1, 1].Value = "Invoice Date"; ws.Cells[1, 2].Value = "Patient"; ws.Cells[1, 3].Value = "Doctor";
            ws.Cells[1, 4].Value = "Total"; ws.Cells[1, 5].Value = "Paid"; ws.Cells[1, 6].Value = "Status";
            int row = 2;
            foreach (var i in invoices)
            {
                ws.Cells[row, 1].Value = i.InvoiceDate.ToString("yyyy-MM-dd");
                ws.Cells[row, 2].Value = i.Patient?.User?.FullName;
                ws.Cells[row, 3].Value = i.Appointment?.Doctor?.User?.FullName;
                ws.Cells[row, 4].Value = i.TotalAmount; ws.Cells[row, 5].Value = i.PaidAmount;
                ws.Cells[row, 6].Value = i.PaymentStatus.ToString();
                row++;
            }
            ws.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }
    }
}
