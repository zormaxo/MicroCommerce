using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering.Infrastructure.Context;

public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContextDesignFactory()
    {
    }

    public OrderDbContext CreateDbContext(string[] args)
    {
        var connStr = "Data Source=c_sqlserver;Initial Catalog=order;Persist Security Info=True;User ID=sa;Password=Salih123!";

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(connStr);

        return new OrderDbContext(optionsBuilder.Options, new NoMediator());
    }

    class NoMediator : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        { return default; }

        public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
        { return default; }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        { return Task.CompletedTask; }

        public Task Publish(object notification, CancellationToken cancellationToken = default) { return Task.CompletedTask; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        { return Task.FromResult<TResponse>(default); }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        { return Task.FromResult<object>(default); }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        { return Task.FromResult<TRequest>(default); }
    }
}
