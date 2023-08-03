namespace RKeeper.Infrastructure.HealthChecks;
public static class HealthCheckPathConstants
{
    public const string ReadinessPath = "/healthz/ready";
    public const string LivenessPath = "/healthz";
    public const string StartupPath = "/healthz/startup";
}

public static class HealthCheckStatusConstants
{
    public const int MinStatusCodeHealthy = 200;
    public const int MaxStatusCodeHealthy = 299;

    public const int MinStatusCodeDegraded = 200;
    public const int MaxStatusCodeDegraded = 399;

    public const int MinStatusCodeUnhealthy = 500;
    public const int MaxStatusCodeUnhealthy = 599;
}

public static class HealthCheckTagConstants
{
    public const string ReadyHealthCheckTag = "ready";
    public const string LiveHealthCheckTag = "live";
    public const string StartupHealthCheckTag = "startup";
}
