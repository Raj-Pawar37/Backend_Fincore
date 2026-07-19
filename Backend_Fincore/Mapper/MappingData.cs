using AutoMapper;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using System.Runtime.CompilerServices;

namespace Backend_Fincore.Mapper
{
    public class MappingData : Profile
    {
        public MappingData() 
        {

            //employee
            CreateMap<Employee, EmployeeReadDTO>()
                .ForMember(d => d.CompanyName, x => x.MapFrom(y => y.Company.CompanyName))
                .ForMember(d => d.DepartmentName,x => x.MapFrom(y => y.Department.DepartmentName))
                .ForMember(d => d.ReportingManagerName,x => x.MapFrom(y => y.ReportingManager != null
                ? y.ReportingManager.FirstName + " " + y.ReportingManager.LastName
                : null));

            CreateMap<Employee, EmployeeWriteDTO>()
                .ReverseMap();

            //user
            CreateMap<User, UserReadDTO>()
                  .ForMember(dest => dest.RoleName,opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<User, UserWriteDTO>().ReverseMap();


            //company
            CreateMap<Company, CompanyReadDTO>()
                .ForMember(d => d.CountryName, x => x.MapFrom(y => y.Country.CountryName))
                .ForMember(d => d.StateName, x => x.MapFrom(y => y.State.StateName))
                .ForMember(d => d.CityName, x => x.MapFrom(y => y.City.CityName));
            CreateMap<CompanyWriteDTO, Company>().ReverseMap();


            //Vendor
            // Vendor
            CreateMap<Vendor, VendorReadDTO>()
                .ForMember(d => d.CompanyName,
                    x => x.MapFrom(y => y.Company.CompanyName));

            CreateMap<Vendor, VendorWriteDTO>()
                .ReverseMap();


        }
      
        
    }
}
