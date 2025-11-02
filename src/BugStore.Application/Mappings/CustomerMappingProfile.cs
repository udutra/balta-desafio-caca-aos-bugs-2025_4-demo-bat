using AutoMapper;
using BugStore.Application.DTOs.Customer;
using BugStore.Domain.Entities;

namespace BugStore.Application.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.StatusCode, opt
                => opt.MapFrom(src => 200))
            .ForMember(dest => dest.Message, opt
                => opt.MapFrom(src => "Sucesso"));
    }
}