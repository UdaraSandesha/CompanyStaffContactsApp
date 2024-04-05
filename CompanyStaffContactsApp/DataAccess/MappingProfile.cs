using AutoMapper;
using CompanyStaffContactsApp.Dtos;
using CompanyStaffContactsApp.Models;

namespace CompanyStaffContactsApp.DataAccess
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Staff, StaffDto>()
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FirstName + " " + src.Manager.LastName : string.Empty))
                .ReverseMap()
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerId, opt => opt.Ignore());
        }
    }
}
