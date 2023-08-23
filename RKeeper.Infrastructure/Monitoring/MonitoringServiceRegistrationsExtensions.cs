using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RKeeper.Infrastructure.Monitoring.OpenTelemetry;

namespace RKeeper.Infrastructure.Monitoring;

public static class MonitoringServiceRegistrationsExtensions
{
    public static WebApplicationBuilder AddRKeeperMonitoring(this WebApplicationBuilder builder, Action<MonitoringOptions>? configureOptions = null)
    {
        AddRKeeperMonitoringCore(builder.Services, builder.Configuration, configureOptions);
        return builder;
    }

    public static IServiceCollection AddRKeeperMonitoring(this IServiceCollection services, IConfiguration configuration, Action<MonitoringOptions>? configureOptions = null)
    {
        AddRKeeperMonitoringCore(services, configuration, configureOptions);
        return services;
    }


    private static void AddRKeeperMonitoringCore(IServiceCollection services, IConfiguration configuration, Action<MonitoringOptions>? configureOptions = null)
    {
        var monitoringConfigurationSection = configuration.GetSection(MonitoringOptions.SectionName);

        services.Configure<MonitoringOptions>(monitoringConfigurationSection);
        var options = CreateMonitoringOptions(monitoringConfigurationSection, configureOptions);
        if (options.OpenTelemetry.IsEnabled)
        {
            services.AddRKeeperOpenTelemetry(options.OpenTelemetry);
        }
    }

    private static void AddRKeeperOpenTelemetry(this IServiceCollection services, OpenTelemetryOptions options)
    {
        if (options.IsEnabled == false)
        {
            return;
        }

        var callingAssembly = Assembly.GetExecutingAssembly();
        var callingAssemblyName = callingAssembly.GetName();

        var serviceName = options.ServiceName ?? callingAssemblyName.Name ?? "unknown";
        var serviceVersion = callingAssemblyName.Version?.ToString() ?? "unknown";

        //var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

        services.AddOpenTelemetry()
            .ConfigureResource(configureResource =>
            {
                configureResource.AddService(serviceName, serviceVersion, serviceInstanceId: Environment.MachineName);
            })
            .WithTracing(builder =>
            {
                builder.SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                builder.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                    otlpOptions.Endpoint = new Uri(options.Endpoint);
                });

                if (options.EnableConsoleExporter)
                {
                    builder.AddConsoleExporter();
                }
            })
            .WithMetrics(builder =>
            {
                builder.AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                builder.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                    otlpOptions.Endpoint = new Uri(options.Endpoint);
                });

                if (options.EnableConsoleExporter)
                {
                    builder.AddConsoleExporter();
                }
            });
    }

    private static MonitoringOptions CreateMonitoringOptions(IConfiguration configuration, Action<MonitoringOptions>? configureOptions)
    {
        var options = new MonitoringOptions();
        configuration.Bind(options);
        configureOptions?.Invoke(options);
        return options;
    }
}
