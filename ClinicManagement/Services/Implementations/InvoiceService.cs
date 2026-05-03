using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;
        public InvoiceService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
            => await _context.Invoices.Include(i => i.Appointment).ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .Include(i => i.Patient).ThenInclude(p => p.User).OrderByDescending(i => i.InvoiceDate).ToListAsync();

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
            => await _context.Invoices.Include(i => i.Appointment).ThenInclude(a => a.Doctor).ThenInclude(d => d.User)
                .Include(i => i.Patient).ThenInclude(p => p.User).FirstOrDefaultAsync(i => i.InvoiceId == id);

        public async Task<IEnumerable<Invoice>> GetInvoicesByPatientAsync(int patientId)
            => await _context.Invoices.Include(i => i.Appointment).Where(i => i.PatientId == patientId).OrderByDescending(i => i.InvoiceDate).ToListAsync();

        public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(PaymentStatus status)
            => await _context.Invoices.Include(i => i.Patient).ThenInclude(p => p.User).Where(i => i.PaymentStatus == status).ToListAsync();

        public async Task CreateInvoiceAsync(int appointmentId)
        {
            if (await _context.Invoices.AnyAsync(i => i.AppointmentId == appointmentId)) return;
            var appt = await _context.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appt == null) return;

            var invoice = new Invoice
            {
                AppointmentId = appointmentId,
                PatientId = appt.PatientId,
                TotalAmount = appt.Doctor.ConsultationFee,
                PaidAmount = 0,
                Discount = 0,
                PaymentStatus = PaymentStatus.Pending,
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30)
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(int invoiceId, decimal paidAmount, string? paymentMethod)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) return;
            invoice.PaidAmount = paidAmount;
            invoice.PaymentMethod = paymentMethod;
            invoice.PaymentStatus = paidAmount >= invoice.TotalAmount - invoice.Discount ? PaymentStatus.Paid :
                                    paidAmount > 0 ? PaymentStatus.PartiallyPaid : PaymentStatus.Pending;
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Invoices.Where(i => i.PaymentStatus == PaymentStatus.Paid);
            if (from.HasValue) query = query.Where(i => i.InvoiceDate >= from.Value);
            if (to.HasValue) query = query.Where(i => i.InvoiceDate <= to.Value);
            return await query.SumAsync(i => i.PaidAmount);
        }

        public async Task<int> GetPendingInvoiceCountAsync()
            => await _context.Invoices.CountAsync(i => i.PaymentStatus == PaymentStatus.Pending);
    }
}
