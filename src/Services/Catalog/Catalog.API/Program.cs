using Catalog.API;
using Catalog.API.Extensions;
using Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCustomDbContext(builder.Configuration);
builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.ConfigureConsul(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();
IServerAddressesFeature addressFeature = null;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MigrateDbContext<CatalogContext>(
    (context, services) =>
    {
        var env = services.GetService<IWebHostEnvironment>();
        var settings = services.GetService<IOptions<CatalogSettings>>();
        var logger = services.GetService<ILogger<CatalogContextSeed>>();

        new CatalogContextSeed().SeedAsync(context, env, settings, logger).Wait();
    });

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Pic")),
        RequestPath = "/Pic"
    });

app.Start();

app.RegisterWithConsul(app.Lifetime, builder.Configuration);

app.WaitForShutdown();