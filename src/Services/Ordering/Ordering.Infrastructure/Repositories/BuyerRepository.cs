using Ordering.Application.Interfaces.Repositories;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.Repositories;

public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
{
    public BuyerRepository(OrderDbContext dbContext) : base(dbContext)
    {
    }
}
