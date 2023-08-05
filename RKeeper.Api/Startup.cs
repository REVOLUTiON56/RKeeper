using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RKeeper.Infrastructure.HealthChecks;
using RKeeper.Infrastructure.Monitoring;

namespace RKeeper.Api;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        builder.AddMonitoringServices();
        builder.AddCommonHealthChecks();

        builder.Services.AddSwaggerGen();
    }


    public static void Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseAuthorization();

        app.MapCommonHealthChecks();
        app.MapControllers();
    }
}
