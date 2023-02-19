using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Basket.API.Extensions;

public static class ConsulRegistration
{
    public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(
            p => new ConsulClient(
                consulConfig =>
                {
                    var address = configuration["ConsulConfig:Address"];
                    consulConfig.Address = new Uri(address);
                }));

        return services;
    }

    public static IApplicationBuilder RegisterWithConsul(
        this IApplicationBuilder app,
        IHostApplicationLifetime lifetime,
        IConfiguration configuration)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

        //var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
        //var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
        //var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

        // Get Server Ip Address
        var features = app.Properties["server.Features"] as FeatureCollection;
        var address = features.Get<IServerAddressesFeature>().Addresses.First();

        // Register service with consul
        var uri = new Uri(address);
        var registration = new AgentServiceRegistration()
        {
            ID = "BasketService",
            Name = "BasketService",
            Address = $"{uri.Host}",
            Port = uri.Port,
            Tags = new[] { "BasketService" }
        };

        logger.LogInformation("Registering with Consul");
        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        lifetime.ApplicationStopping
            .Register(
                () =>
                {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });

        return app;
    }
}
