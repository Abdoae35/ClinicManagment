using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService) => _invoiceService = invoiceService;

        public async Task<IActionResult> Index(int? status)
        {
            var invoices = status.HasValue
                ? await _invoiceService.GetInvoicesByStatusAsync((PaymentStatus)status.Value)
                : await _invoiceService.GetAllInvoicesAsync();
            ViewBag.Status = status;
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id, decimal paidAmount, string? paymentMethod)
        {
            await _invoiceService.UpdatePaymentAsync(id, paidAmount, paymentMethod);
            TempData["Success"] = "Payment updated successfully.";
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Print(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }
    }
}
