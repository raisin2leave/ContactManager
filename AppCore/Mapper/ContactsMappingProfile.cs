using AppCore.Dto;
using AppCore.Entities;
using AutoMapper;

namespace AppCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.EmployerName, opt => opt.MapFrom(src => src.Employer != null ? src.Employer.Name : null));

        CreateMap<CreatePersonDto, Person>();
        CreateMap<UpdatePersonDto, Person>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Address, AddressDto>().ReverseMap();
    }
}