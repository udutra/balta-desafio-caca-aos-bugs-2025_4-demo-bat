using AutoMapper;
using BugStore.Application.DTOs.Order;
using BugStore.Application.DTOs.OrderLine;
using BugStore.Domain.Entities;

namespace BugStore.Application.Mappings;

public class OrderMappingProfile : Profile{
    public OrderMappingProfile(){
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 200))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Sucesso"));

        CreateMap<OrderLine, OrderLineDto>();
    }
}