using AutoMapper;
using CRM_Lite.Data.Dtos;
using CRM_Lite.Data.Models;

namespace CRM_Lite.AutoMapper
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>()
                .ReverseMap();
        }
    }
}