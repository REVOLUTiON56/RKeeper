namespace RKeeper.Infrastructure.HealthChecks;

public class HealthCheckPathsOptions
{
    public string StartupPath { get; set; } = HealthCheckPathConstants.StartupPath;
    public string LivenessPath { get; set; } = HealthCheckPathConstants.LivenessPath;
    public string ReadinessPath { get; set; } = HealthCheckPathConstants.ReadinessPath;
}

