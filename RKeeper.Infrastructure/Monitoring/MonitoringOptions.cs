using RKeeper.Infrastructure.Monitoring.OpenTelemetry;

namespace RKeeper.Infrastructure.Monitoring;

public class MonitoringOptions
{
    public const string SectionName = "Monitoring";
    public OpenTelemetryOptions OpenTelemetry { get; set; } = null!;
}
