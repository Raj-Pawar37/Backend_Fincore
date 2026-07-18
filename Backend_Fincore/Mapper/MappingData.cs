using AutoMapper;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;

namespace Backend_Fincore.Mapper
{
    public class MappingData : Profile
    {

        public MappingData()
        {
            // < src , dest >
            //CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<RoleDTO, Role>();
               

        }
    }
}
