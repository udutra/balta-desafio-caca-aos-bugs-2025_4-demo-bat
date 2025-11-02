using AutoMapper;
using BugStore.Application.DTOs.Product;
using BugStore.Domain.Entities;

namespace BugStore.Application.Mappings;

public class ProductMappingProfile : Profile{
    public ProductMappingProfile(){
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 200))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Sucesso"));
    }
}