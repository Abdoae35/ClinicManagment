using AutoMapper;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.ViewModels;

namespace ClinicManagement.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Doctor, DoctorCreateViewModel>().ReverseMap();
            CreateMap<Doctor, DoctorEditViewModel>().ReverseMap();
            CreateMap<Patient, PatientCreateViewModel>().ReverseMap();
            CreateMap<Patient, PatientEditViewModel>().ReverseMap();
            CreateMap<Appointment, AppointmentCreateViewModel>().ReverseMap();
            CreateMap<Appointment, AppointmentEditViewModel>().ReverseMap();
            CreateMap<Clinic, ClinicCreateViewModel>().ReverseMap();
            CreateMap<Clinic, ClinicEditViewModel>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordCreateViewModel>().ReverseMap();
        }
    }
}
