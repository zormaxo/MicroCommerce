using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Web.ApiGateway.Extensions;
using Web.ApiGateway.Infrastructure;
using Web.ApiGateway.Services;
using Web.ApiGateway.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();

builder.Configuration.AddJsonFile("Configurations/ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddConsul();
builder.Services.ConfigureAuth(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddCors(
        options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                builder => builder.SetIsOriginAllowed((host) => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IBasketService, BasketService>();

ConfigureHttpClient(builder.Services);

var app = builder.Build();
app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.UseOcelot();

app.Run();


void ConfigureHttpClient(IServiceCollection services)
{
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddTransient<HttpClientDelegatingHandler>();

    services.AddHttpClient(
        "basket",
        c =>
        {
            c.BaseAddress = new Uri(builder.Configuration["urls:basket"]);
        })
        .AddHttpMessageHandler<HttpClientDelegatingHandler>();

    services.AddHttpClient(
        "catalog",
        c =>
        {
            c.BaseAddress = new Uri(builder.Configuration["urls:catalog"]);
        })
        .AddHttpMessageHandler<HttpClientDelegatingHandler>();
}
