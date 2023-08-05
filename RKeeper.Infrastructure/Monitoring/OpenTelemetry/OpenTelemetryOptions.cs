namespace RKeeper.Infrastructure.Monitoring.OpenTelemetry;

public class OpenTelemetryOptions
{
    public bool IsEnabled { get; set; } = true;
    public bool EnableConsoleExporter { get; set; }
    public string Endpoint { get; set; } = null!;
    public string? ServiceName { get; set; }
}
