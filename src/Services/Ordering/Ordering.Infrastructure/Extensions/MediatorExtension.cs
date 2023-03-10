using MediatR;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.Extensions;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
