using EventBus;
using EventBus.Abstraction;
using Ordering.API.Extensions;
using Ordering.API.Extensions.Registration.EventHandlerRegistration;
using Ordering.API.Extensions.Registration.ServiceDiscovery;
using Ordering.API.IntegrationEvents.EventHandlers;
using Ordering.API.IntegrationEvents.Events;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Context;
using Serilog;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Configurations/appsettings.json", optional: false)
    .AddJsonFile($"Configurations/appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var serilogConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Configurations/serilog.json", optional: false)
    .AddJsonFile($"Configurations/serilog.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .UseDefaultServiceProvider(
        (context, options) =>
        {
            options.ValidateOnBuild = false;
            options.ValidateScopes = false;
        });
builder.Configuration.AddConfiguration(configuration);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>();
builder.Services.AddScoped<OrderDbContextSeed>();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

ConfigureService(builder.Services);
var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom
    .Configuration(serilogConfiguration)
    .CreateLogger();

app.MigrateDbContext<OrderDbContext>(
    (context, services) =>
    {
        var logger = services.GetService<ILogger<OrderDbContext>>();

        var dbContextSeeder = services.GetService<OrderDbContextSeed>();
        dbContextSeeder.SeedAsync(context, logger).Wait();
    });

Log.Information("Application is Running....");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ConfigureEventBusForSubscription(app);

app.Run();


void ConfigureService(IServiceCollection services)
{
    services
        .AddLogging(configure => configure.AddConsole())
        .AddApplicationRegistration(typeof(Program))
        .AddPersistenceRegistration(builder.Configuration)
        .ConfigureEventHandlers()
        .AddServiceDiscoveryRegistration(builder.Configuration);

    services.AddSingleton(
        sp =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",
                SubscriberClientAppName = "OrderService",
                //Connection =
                //    new ConnectionFactory() { HostName = "localhost", Port = 15672, UserName = "guest", Password = "guest" },
                //Connection = new ConnectionFactory()
                //{
                //    //HostName = "c_rabbitmq"
                //    HostName = "http://localhost:15672"
                //},
                EventBusType = EventBusType.RabbitMQ,
            };

            return EventBusFactory.EventBusFactory.Create(config, sp);
        });
}

void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}