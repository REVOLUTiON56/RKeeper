using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RKeeper.Infrastructure.HealthChecks;

public static class HealthCheckResponseWriter
{
    static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static Task WriteResponse(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var status = result.Status.ToString().ToUpperInvariant();
        var criticalDependencies = result.Entries.ToDictionary(
            p => p.Key,
            p => new CriticalDependencyEntry(p.Value.Status.ToString().ToUpperInvariant()));

        var response = new HealthResponse(status, criticalDependencies);

        var json = JsonSerializer.Serialize(response, DefaultOptions);
        return context.Response.WriteAsync(json);
    }

    record HealthResponse(string Status, Dictionary<string, CriticalDependencyEntry> CriticalDependencies);
    record CriticalDependencyEntry(string Status);
}
