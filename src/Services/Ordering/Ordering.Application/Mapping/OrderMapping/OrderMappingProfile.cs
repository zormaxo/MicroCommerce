using AutoMapper;
using Ordering.Application.Features.Commands.CreateOrder;
using Ordering.Application.Features.Queries.ViewModels;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Application.Mapping.OrderMapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, CreateOrderCommand>().ReverseMap();

        CreateMap<OrderItem, OrderItemDTO>().ReverseMap();

        CreateMap<Order, OrderDetailViewModel>()
            .ForMember(x => x.City, y => y.MapFrom(z => z.Address.City))
            .ForMember(x => x.Country, y => y.MapFrom(z => z.Address.Country))
            .ForMember(x => x.Street, y => y.MapFrom(z => z.Address.Street))
            .ForMember(x => x.Zipcode, y => y.MapFrom(z => z.Address.ZipCode))
            .ForMember(x => x.Date, y => y.MapFrom(z => z.OrderDate))
            .ForMember(x => x.Ordernumber, y => y.MapFrom(z => z.Id.ToString()))
            .ForMember(x => x.Status, y => y.MapFrom(z => z.OrderStatus.Name))
            .ForMember(x => x.Total, y => y.MapFrom(z => z.OrderItems.Sum(i => i.Units * i.UnitPrice)))
            .ReverseMap();

        CreateMap<OrderItem, Orderitem>();
    }
}
