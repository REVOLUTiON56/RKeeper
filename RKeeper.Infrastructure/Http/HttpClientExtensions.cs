using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace RKeeper.Infrastructure.Http;

public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddDefaultPolicies<T>(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder
            .AddDefaultRetryPolicy<T>()
            .AddDefaultCircuitBreakerPolicy();
    }

    public static IHttpClientBuilder AddDefaultRetryPolicy<T>(
        this IHttpClientBuilder httpClientBuilder,
        Action<IAsyncPolicy<HttpResponseMessage>>? configure = null)
    {

        httpClientBuilder.AddPolicyHandler((provider, _) => {
            var policy = DefaultRetryPolicy<T>(provider);
            configure?.Invoke(policy);
            return policy;
        });

        return httpClientBuilder;
    }

    public static IHttpClientBuilder AddDefaultCircuitBreakerPolicy(
        this IHttpClientBuilder httpClientBuilder,
        Action<IAsyncPolicy<HttpResponseMessage>>? configure = null)
    {

        httpClientBuilder.AddPolicyHandler((_, _) => {
            var policy = DefaultCircuitBreakerPolicy;
            configure?.Invoke(policy);
            return policy;
        });

        return httpClientBuilder;
    }

    public static IAsyncPolicy<HttpResponseMessage> DefaultRetryPolicy<T>(IServiceProvider provider) => HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryAttempt, _) => {
                    var request = outcome.Result.RequestMessage;
                    var url = (request?.Method.ToString() ?? string.Empty) + " " + (request?.RequestUri?.ToString() ?? string.Empty);

                    var logger = provider.GetRequiredService<ILogger<T>>();
                    logger.LogWarning("An error occurred while executing the request {Url} - ({StatusCode}). Waiting {Delay} ms, before retry #{Retry}", url, outcome.Result.StatusCode, timespan.TotalMilliseconds, retryAttempt);
                });

    public static IAsyncPolicy<HttpResponseMessage> DefaultCircuitBreakerPolicy => HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
