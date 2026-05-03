using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Models.Entities;

namespace ClinicManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // --- ApplicationUser ---
            b.Entity<ApplicationUser>(e => {
                e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.IsActive).HasDefaultValue(true);
            });

            // --- Doctor ---
            b.Entity<Doctor>(e => {
                e.HasKey(x => x.DoctorId);
                e.HasOne(x => x.User).WithOne(u => u.Doctor).HasForeignKey<Doctor>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Specialization).WithMany(s => s.Doctors).HasForeignKey(x => x.SpecializationId).OnDelete(DeleteBehavior.Restrict);
                e.Property(x => x.ConsultationFee).HasColumnType("decimal(18,2)");
                e.HasIndex(x => x.LicenseNumber).IsUnique();
                e.HasIndex(x => x.UserId).IsUnique();
            });

            // --- Specialization ---
            b.Entity<Specialization>(e => {
                e.HasKey(x => x.SpecializationId);
                e.HasIndex(x => x.Name).IsUnique();
            });

            // --- Patient ---
            b.Entity<Patient>(e => {
                e.HasKey(x => x.PatientId);
                e.HasOne(x => x.User).WithOne(u => u.Patient).HasForeignKey<Patient>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => x.UserId).IsUnique();
            });

            // --- Appointment ---
            b.Entity<Appointment>(e => {
                e.HasKey(x => x.AppointmentId);
                e.HasOne(x => x.Patient).WithMany(p => p.Appointments).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Doctor).WithMany(d => d.Appointments).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Clinic).WithMany(c => c.Appointments).HasForeignKey(x => x.ClinicId).OnDelete(DeleteBehavior.SetNull);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasIndex(x => new { x.DoctorId, x.AppointmentDate, x.AppointmentTime });
                e.HasIndex(x => x.Status);
            });

            // --- Clinic ---
            b.Entity<Clinic>(e => {
                e.HasKey(x => x.ClinicId);
                e.HasOne(x => x.Specialization).WithMany(s => s.Clinics).HasForeignKey(x => x.SpecializationId).OnDelete(DeleteBehavior.SetNull);
            });

            // --- MedicalRecord ---
            b.Entity<MedicalRecord>(e => {
                e.HasKey(x => x.RecordId);
                e.HasOne(x => x.Patient).WithMany(p => p.MedicalRecords).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Doctor).WithMany(d => d.MedicalRecords).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Appointment).WithOne(a => a.MedicalRecord).HasForeignKey<MedicalRecord>(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasIndex(x => x.AppointmentId).IsUnique();
            });

            // --- PrescriptionItem ---
            b.Entity<PrescriptionItem>(e => {
                e.HasKey(x => x.PrescriptionId);
                e.HasOne(x => x.MedicalRecord).WithMany(m => m.PrescriptionItems).HasForeignKey(x => x.RecordId).OnDelete(DeleteBehavior.Cascade);
            });

            // --- Invoice ---
            b.Entity<Invoice>(e => {
                e.HasKey(x => x.InvoiceId);
                e.HasOne(x => x.Appointment).WithOne(a => a.Invoice).HasForeignKey<Invoice>(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Patient).WithMany(p => p.Invoices).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
                e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
                e.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Discount).HasColumnType("decimal(18,2)");
                e.HasIndex(x => x.AppointmentId).IsUnique();
            });

            // --- DoctorSchedule ---
            b.Entity<DoctorSchedule>(e => {
                e.HasKey(x => x.ScheduleId);
                e.HasOne(x => x.Doctor).WithMany(d => d.Schedules).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => new { x.DoctorId, x.DayOfWeek });
            });

            // --- Notification ---
            b.Entity<Notification>(e => {
                e.HasKey(x => x.NotificationId);
                e.HasOne(x => x.User).WithMany(u => u.Notifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasIndex(x => new { x.UserId, x.IsRead });
            });

            // --- AuditLog ---
            b.Entity<AuditLog>(e => {
                e.HasKey(x => x.LogId);
                e.Property(x => x.Timestamp).HasDefaultValueSql("GETUTCDATE()");
                e.HasIndex(x => x.Timestamp);
            });
        }
    }
}
