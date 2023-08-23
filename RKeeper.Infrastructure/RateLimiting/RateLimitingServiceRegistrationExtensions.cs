using Microsoft.AspNetCore.Builder;

namespace RKeeper.Infrastructure.RateLimiting;

public static class RateLimitingServiceRegistrationExtensions
{
    public static WebApplicationBuilder AddRKeeperRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.AddDefaultPolicy();
        });

        return builder;
    }

    public static WebApplication UseRKeeperRateLimiting(this WebApplication app)
    {
        app.UseRateLimiter();
        return app;
    }
}
