using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RKeeper.Infrastructure.HealthChecks;

public static class HealthCheckEndpointsExtensions
{
    public static IEndpointConventionBuilder MapLiveness(this IEndpointRouteBuilder endpoints, string pattern)
        => endpoints.MapHealthChecks(
            pattern,
            GetDefaultOptions(HealthCheckTagConstants.LiveHealthCheckTag));

    public static IEndpointConventionBuilder MapReadiness(this IEndpointRouteBuilder endpoints, string pattern)
        => endpoints.MapHealthChecks(
            pattern,
            GetDefaultOptions(HealthCheckTagConstants.ReadyHealthCheckTag));

    public static IEndpointConventionBuilder MapStartup(this IEndpointRouteBuilder endpoints, string pattern)
        => endpoints.MapHealthChecks(
            pattern,
            GetDefaultOptions(HealthCheckTagConstants.StartupHealthCheckTag));

    public static IEndpointRouteBuilder MapRKeeperHealthChecks(
        this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapRKeeperHealthChecks(_ => { });
    }

    public static IEndpointRouteBuilder MapRKeeperHealthChecks(this IEndpointRouteBuilder endpoints, string[]? hosts, Action<HealthCheckPathsOptions>? configure = null)
    {
        var requiredHosts = hosts ?? Array.Empty<string>();
        endpoints.MapRKeeperHealthChecks(
            configure,
            startup => startup.RequireHost(requiredHosts),
            liveness => liveness.RequireHost(requiredHosts),
            readiness => readiness.RequireHost(requiredHosts)
        );

        return endpoints;
    }

    public static IEndpointRouteBuilder MapRKeeperHealthChecks(
        this IEndpointRouteBuilder endpoints,
        Action<HealthCheckPathsOptions>? configure,
        Action<IEndpointConventionBuilder>? startupOptions = null,
        Action<IEndpointConventionBuilder>? livenessOptions = null,
        Action<IEndpointConventionBuilder>? readinessOptions = null)
    {
        var pathOptions = new HealthCheckPathsOptions();
        configure?.Invoke(pathOptions);

        var builder = endpoints.MapStartup(pathOptions.StartupPath);
        startupOptions?.Invoke(builder);

        builder = endpoints.MapLiveness(pathOptions.LivenessPath);
        livenessOptions?.Invoke(builder);

        builder = endpoints.MapReadiness(pathOptions.ReadinessPath);
        readinessOptions?.Invoke(builder);

        return endpoints;
    }

    private static HealthCheckOptions GetDefaultOptions(string tag)
    {
        return new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(tag),
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status203NonAuthoritative,
                [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError,
            },
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        };
    }

}
