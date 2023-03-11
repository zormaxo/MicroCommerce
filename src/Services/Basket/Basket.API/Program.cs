using Basket.API.Extensions;
using Basket.API.Infrastructure.Repositories;
using Basket.API.IntegrationEvents.EventHandling;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using Basket.API.Services;
using EventBus;
using EventBus.Abstraction;
using Serilog;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var serilogConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"serilog.json", optional: false)
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));
builder.Services
    .AddSingleton<IEventBus>(
        serviceProvider =>
        {
            EventBusConfig eventBusConfig = new()
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",
                SubscriberClientAppName = "BasketService",
                EventBusType = EventBusType.RabbitMQ,
            };

            return EventBusFactory.EventBusFactory.Create(eventBusConfig, serviceProvider);
        });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(Log.Logger);

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom
    .Configuration(serilogConfiguration)
    .CreateLogger();

ConfigureEventBus(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("Basket is Running....");

app.Start();

app.RegisterWithConsul(app.Lifetime, builder.Configuration);

app.WaitForShutdown();

static void ConfigureEventBus(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}
