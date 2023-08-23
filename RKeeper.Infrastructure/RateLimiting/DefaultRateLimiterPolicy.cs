using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace RKeeper.Infrastructure.RateLimiting;

public class DefaultRateLimiterPolicy : IRateLimiterPolicy<string>
{
    public const string Name = "default";

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

        return ValueTask.CompletedTask;
    };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        // same policy name and same partition key => will use the same rate limiter instance
        var (isAuthenticated, partitionKey) = GetParameters();
        // SlidingWindowRateLimiter doesn't set RetryAfter header :sad-face:
        return RateLimitPartition.GetTokenBucketLimiter(
            partitionKey,
            _ => CreateTokenBucketLimiterOptions(isAuthenticated));

        (bool isAuthenticated, string partitionKey) GetParameters()
        {
            return httpContext.User.Identity?.IsAuthenticated == true
                ? (true, httpContext.User.Identity.Name!)
                : (false, httpContext.Request.Headers.Host.ToString());
        }
    }


    public static TokenBucketRateLimiterOptions CreateTokenBucketLimiterOptions(bool isUserAuthenticated)
    {
        return new TokenBucketRateLimiterOptions
        {
            AutoReplenishment = true,
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            TokenLimit = 10,
            TokensPerPeriod = 1,
            ReplenishmentPeriod = isUserAuthenticated
                ? TimeSpan.FromSeconds(10)
                : TimeSpan.FromSeconds(30)
        };
    }
}

public static class DefaultRateLimiterPolicyExtensions
{
    public static RateLimiterOptions AddDefaultPolicy(this RateLimiterOptions rateLimiterOptions)
    {
        return rateLimiterOptions.AddPolicy<string, DefaultRateLimiterPolicy>(DefaultRateLimiterPolicy.Name);
    }
}
