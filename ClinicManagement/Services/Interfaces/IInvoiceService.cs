using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetInvoicesByPatientAsync(int patientId);
        Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(PaymentStatus status);
        /// <summary>Auto-creates an invoice from a completed appointment.</summary>
        Task CreateInvoiceAsync(int appointmentId);
        Task UpdatePaymentAsync(int invoiceId, decimal paidAmount, string? paymentMethod);
        Task<decimal> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null);
        Task<int> GetPendingInvoiceCountAsync();
    }
}
