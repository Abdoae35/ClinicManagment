namespace ClinicManagement.Services.Interfaces
{
    public interface IReportService
    {
        Task<object> GetAppointmentReportAsync(DateTime? from, DateTime? to, int? doctorId, int? status);
        Task<object> GetRevenueReportAsync(DateTime? from, DateTime? to, int? doctorId);
        Task<byte[]> ExportAppointmentReportToExcelAsync(DateTime? from, DateTime? to, int? doctorId, int? status);
        Task<byte[]> ExportRevenueReportToExcelAsync(DateTime? from, DateTime? to, int? doctorId);
    }
}
