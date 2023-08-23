using HealthChecks.NpgSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace RKeeper.Infrastructure.HealthChecks;

public static class HealthCheckServiceRegistrationsExtensions
{
    const string HealthCheckQuery = "SELECT 1;";
    const string DefaultPSqlHealthCheckName = "postgres";

    public static void AddRKeeperHealthChecks(this WebApplicationBuilder builder, Action<IHealthChecksBuilder>? builderAction = null)
    {
        builder.Services.AddRKeeperHealthChecks(builderAction);
    }

    public static void AddRKeeperHealthChecks(this IServiceCollection services, Action<IHealthChecksBuilder>? builderAction = null)
    {
        var builder = services.AddHealthChecks();
        builderAction?.Invoke(builder);
    }

    /// <summary>
    /// Add postgres database health check from connection string section
    /// </summary>
    /// <param name="builder">A builder used to register health checks</param>
    /// <param name="connectionStringConfigurationKey">The key with connection string in the Configuration::ConnectionStrings section</param>
    /// <param name="healthCheckName">The health check name</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static IHealthChecksBuilder AddDatabaseHealthCheckFromConnectionStrings(this IHealthChecksBuilder builder, string connectionStringConfigurationKey, string? healthCheckName = null)
    {
        return builder.AddDatabaseHealthCheck(configuration => configuration.GetConnectionString(connectionStringConfigurationKey)!, healthCheckName ?? connectionStringConfigurationKey);
    }

    /// <summary>
    /// Add postgres database health check from connection string section
    /// </summary>
    /// <param name="builder">A builder used to register health checks</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="healthCheckName">The health check name</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static IHealthChecksBuilder AddDatabaseHealthCheck(this IHealthChecksBuilder builder, string connectionString, string? healthCheckName)
    {
        return builder.AddDatabaseHealthCheck(_ => connectionString, healthCheckName ?? DefaultPSqlHealthCheckName);
    }

    /// <summary>
    /// Add postgres database health check from connection string section
    /// </summary>
    /// <param name="builder">A builder used to register health checks</param>
    /// <param name="getConnectionString">Function to get database connection string from IConfiguration</param>
    /// <param name="healthCheckName">The health check name</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static IHealthChecksBuilder AddDatabaseHealthCheck(this IHealthChecksBuilder builder, Func<IConfiguration, string> getConnectionString, string? healthCheckName = null)
    {
        return builder.AddPSqlHealthCheck(getConnectionString, healthCheckName ?? DefaultPSqlHealthCheckName);
    }

    public static IHealthChecksBuilder AddPSqlHealthCheck(
        this IHealthChecksBuilder builder,
        string connectionString,
        string name = DefaultPSqlHealthCheckName,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {

        return AddPSqlHealthCheck(builder, _ => connectionString, name, failureStatus, tags);
    }

    public static IHealthChecksBuilder AddPSqlHealthCheck(
        this IHealthChecksBuilder builder,
        Func<IConfiguration, string> configurationFunc,
        string name = DefaultPSqlHealthCheckName,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {
        var hTags = tags ?? new[] { HealthCheckTagConstants.StartupHealthCheckTag, HealthCheckTagConstants.ReadyHealthCheckTag };
        var hFailureStatus = failureStatus ?? HealthStatus.Unhealthy;

        var registration = new HealthCheckRegistration(name, sp => CreateHealthCheck(sp, configurationFunc), hFailureStatus, hTags);
        builder.Add(registration);

        return builder;
    }

    static NpgSqlHealthCheck CreateHealthCheck(IServiceProvider sp, Func<IConfiguration, string> configurationFunc)
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var connectionString = configurationFunc(configuration) ?? throw new ArgumentException("Connection string must be specified");

        var options = new NpgSqlHealthCheckOptions {
            DataSource = NpgsqlDataSource.Create(connectionString),
            CommandText = HealthCheckQuery
        };

        return new NpgSqlHealthCheck(options);
    }
}
