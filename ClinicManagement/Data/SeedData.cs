using Microsoft.AspNetCore.Identity;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin
            if (await userManager.FindByEmailAsync("admin@hospital.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@hospital.com",
                    Email = "admin@hospital.com",
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    PhoneNumber = "0500000001",
                    Gender = Gender.Male,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(admin, "Admin@123456");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Receptionist
            if (await userManager.FindByEmailAsync("reception@hospital.com") == null)
            {
                var receptionist = new ApplicationUser
                {
                    UserName = "reception@hospital.com",
                    Email = "reception@hospital.com",
                    FullName = "Sara Reception",
                    EmailConfirmed = true,
                    PhoneNumber = "0500000002",
                    Gender = Gender.Female,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(receptionist, "Recept@123456");
                await userManager.AddToRoleAsync(receptionist, "Receptionist");
            }

            // Seed Specializations
            if (!context.Specializations.Any())
            {
                var specs = new List<Specialization>
                {
                    new() { Name = "Cardiology", Description = "Heart and cardiovascular system", IconClass = "bi-heart-pulse" },
                    new() { Name = "Dermatology", Description = "Skin, hair, and nails", IconClass = "bi-brightness-high" },
                    new() { Name = "Orthopedics", Description = "Bones, joints, and muscles", IconClass = "bi-bandaid" }
                };
                context.Specializations.AddRange(specs);
                await context.SaveChangesAsync();
            }

            var specializations = context.Specializations.ToList();

            // Seed Clinics
            if (!context.Clinics.Any())
            {
                var clinics = new List<Clinic>
                {
                    new() { Name = "Cardiology Clinic A", Floor = "2", RoomNumber = "201", Phone = "1001", SpecializationId = specializations[0].SpecializationId, IsActive = true },
                    new() { Name = "Dermatology Clinic B", Floor = "3", RoomNumber = "302", Phone = "1002", SpecializationId = specializations[1].SpecializationId, IsActive = true },
                    new() { Name = "Orthopedics Clinic C", Floor = "1", RoomNumber = "105", Phone = "1003", SpecializationId = specializations[2].SpecializationId, IsActive = true }
                };
                context.Clinics.AddRange(clinics);
                await context.SaveChangesAsync();
            }

            // Seed Doctors (5)
            string[] doctorEmails = { "doctor1@hospital.com", "doctor2@hospital.com", "doctor3@hospital.com", "doctor4@hospital.com", "doctor5@hospital.com" };
            string[] doctorNames = { "Dr. Ahmed Hassan", "Dr. Fatima Ali", "Dr. Omar Khalid", "Dr. Layla Nasser", "Dr. Yusuf Saeed" };
            int[] specIndexes = { 0, 1, 2, 0, 1 };
            string[] licenses = { "LIC-001", "LIC-002", "LIC-003", "LIC-004", "LIC-005" };
            decimal[] fees = { 200, 180, 250, 220, 190 };
            int[] experience = { 12, 8, 15, 10, 6 };

            for (int i = 0; i < 5; i++)
            {
                if (await userManager.FindByEmailAsync(doctorEmails[i]) == null)
                {
                    var docUser = new ApplicationUser
                    {
                        UserName = doctorEmails[i],
                        Email = doctorEmails[i],
                        FullName = doctorNames[i],
                        EmailConfirmed = true,
                        PhoneNumber = $"050000100{i + 1}",
                        Gender = i % 2 == 0 ? Gender.Male : Gender.Female,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await userManager.CreateAsync(docUser, "Doctor@123456");
                    await userManager.AddToRoleAsync(docUser, "Doctor");

                    if (!context.Doctors.Any(d => d.UserId == docUser.Id))
                    {
                        var doctor = new Doctor
                        {
                            UserId = docUser.Id,
                            SpecializationId = specializations[specIndexes[i]].SpecializationId,
                            LicenseNumber = licenses[i],
                            YearsOfExperience = experience[i],
                            Bio = $"{doctorNames[i]} is a specialist with {experience[i]} years of experience.",
                            ConsultationFee = fees[i],
                            IsAvailable = true,
                            WorkingDays = "Monday,Tuesday,Wednesday,Thursday"
                        };
                        context.Doctors.Add(doctor);
                        await context.SaveChangesAsync();

                        // Add schedule for each doctor
                        var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday };
                        foreach (var day in days)
                        {
                            context.DoctorSchedules.Add(new DoctorSchedule
                            {
                                DoctorId = doctor.DoctorId,
                                DayOfWeek = day,
                                StartTime = new TimeSpan(9, 0, 0),
                                EndTime = new TimeSpan(17, 0, 0),
                                SlotDurationMinutes = 30,
                                IsActive = true
                            });
                        }
                        await context.SaveChangesAsync();
                    }
                }
            }

            // Seed 20 Patients
            for (int i = 1; i <= 20; i++)
            {
                var email = $"patient{i}@hospital.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var patUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = $"Patient {i}",
                        EmailConfirmed = true,
                        PhoneNumber = $"055000{i:D4}",
                        Gender = i % 2 == 0 ? Gender.Female : Gender.Male,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await userManager.CreateAsync(patUser, "Patient@123456");
                    await userManager.AddToRoleAsync(patUser, "Patient");

                    if (!context.Patients.Any(p => p.UserId == patUser.Id))
                    {
                        context.Patients.Add(new Patient
                        {
                            UserId = patUser.Id,
                            BloodType = (BloodType)(i % 8),
                            EmergencyContactName = $"Emergency Contact {i}",
                            EmergencyContactPhone = $"056000{i:D4}",
                            InsuranceNumber = $"INS-{i:D5}",
                            InsuranceProvider = i % 2 == 0 ? "MedInsure" : "HealthPlus",
                            MedicalHistory = i % 3 == 0 ? "No significant history" : null
                        });
                    }
                }
            }
            await context.SaveChangesAsync();

            // Seed 50 Appointments
            if (!context.Appointments.Any())
            {
                var doctors = context.Doctors.ToList();
                var patients = context.Patients.ToList();
                var clinics = context.Clinics.ToList();
                var statuses = new[] { AppointmentStatus.Scheduled, AppointmentStatus.Confirmed, AppointmentStatus.Completed, AppointmentStatus.Cancelled, AppointmentStatus.NoShow };
                var reasons = new[] { "Routine checkup", "Follow-up visit", "Chest pain", "Skin rash", "Joint pain", "Annual exam", "Consultation", "Lab review" };
                var rng = new Random(42);

                for (int i = 0; i < 50; i++)
                {
                    var doctor = doctors[i % doctors.Count];
                    var patient = patients[i % patients.Count];
                    var clinic = clinics[i % clinics.Count];
                    var date = DateTime.Today.AddDays(rng.Next(-30, 15));
                    var hour = 9 + rng.Next(0, 8);
                    var status = statuses[i % statuses.Length];

                    // Past appointments should be completed or cancelled
                    if (date < DateTime.Today)
                        status = i % 3 == 0 ? AppointmentStatus.Completed : (i % 5 == 0 ? AppointmentStatus.Cancelled : AppointmentStatus.Completed);
                    else if (date == DateTime.Today)
                        status = AppointmentStatus.Confirmed;
                    else
                        status = i % 4 == 0 ? AppointmentStatus.Scheduled : AppointmentStatus.Confirmed;

                    var appt = new Appointment
                    {
                        PatientId = patient.PatientId,
                        DoctorId = doctor.DoctorId,
                        ClinicId = clinic.ClinicId,
                        AppointmentDate = date,
                        AppointmentTime = new TimeSpan(hour, (i % 2) * 30, 0),
                        DurationMinutes = 30,
                        Status = status,
                        ReasonForVisit = reasons[i % reasons.Length],
                        Notes = i % 3 == 0 ? "Patient arrived on time" : null,
                        CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 45))
                    };
                    context.Appointments.Add(appt);
                }
                await context.SaveChangesAsync();

                // Create invoices for completed appointments
                var completedAppts = context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .ToList();

                foreach (var appt in completedAppts)
                {
                    var doc = doctors.First(d => d.DoctorId == appt.DoctorId);
                    if (!context.Invoices.Any(inv => inv.AppointmentId == appt.AppointmentId))
                    {
                        context.Invoices.Add(new Invoice
                        {
                            AppointmentId = appt.AppointmentId,
                            PatientId = appt.PatientId,
                            TotalAmount = doc.ConsultationFee,
                            PaidAmount = rng.Next(0, 2) == 1 ? doc.ConsultationFee : 0,
                            Discount = 0,
                            PaymentStatus = rng.Next(0, 2) == 1 ? PaymentStatus.Paid : PaymentStatus.Pending,
                            PaymentMethod = rng.Next(0, 2) == 1 ? "Cash" : "Card",
                            InvoiceDate = appt.AppointmentDate,
                            DueDate = appt.AppointmentDate.AddDays(30)
                        });
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
