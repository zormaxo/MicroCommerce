using AutoMapper;
using MediatR;
using Ordering.Application.Features.Queries.ViewModels;
using Ordering.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Queries.GetOrderDetailById;

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailViewModel>
{
    IOrderRepository orderRepository;
    private readonly IMapper mapper;

    public GetOrderDetailsQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        this.mapper = mapper;
    }

    public async Task<OrderDetailViewModel> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, i => i.OrderItems);

        var result = mapper.Map<OrderDetailViewModel>(order);

        return result;
    }
}
