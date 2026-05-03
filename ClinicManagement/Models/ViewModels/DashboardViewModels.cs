using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int AvailableDoctors { get; set; }
        public int PendingInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<Appointment> TodayAppointmentsList { get; set; } = new List<Appointment>();
        public Dictionary<string, int> AppointmentsPerDay { get; set; } = new();
        public Dictionary<string, int> AppointmentsByStatus { get; set; } = new();
        public Dictionary<string, int> PatientsBySpecialization { get; set; } = new();
    }

    public class DoctorDashboardViewModel
    {
        public int TodayAppointments { get; set; }
        public int PendingRecords { get; set; }
        public IEnumerable<Appointment> TodayAppointmentsList { get; set; } = new List<Appointment>();
        public IEnumerable<DoctorSchedule> WeekSchedule { get; set; } = new List<DoctorSchedule>();
    }
}
