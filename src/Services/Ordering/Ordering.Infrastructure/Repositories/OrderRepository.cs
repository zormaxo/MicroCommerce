using Ordering.Application.Interfaces.Repositories;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Context;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly OrderDbContext dbContext;

    public OrderRepository(OrderDbContext dbContext) : base(dbContext) { this.dbContext = dbContext; }

    public override async Task<Order> GetByIdAsync(Guid id, params Expression<Func<Order, object>>[] includes)
    {
        var entity = await base.GetByIdAsync(id, includes);

        if (entity == null)
        {
            entity = dbContext.Orders.Local.FirstOrDefault(i => i.Id == id);
        }

        return entity;
    }
}
