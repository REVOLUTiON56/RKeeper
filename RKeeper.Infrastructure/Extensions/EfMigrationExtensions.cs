using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RKeeper.Infrastructure.Extensions;

public static class EfMigrationExtensions
{
    /// <summary>
    /// Apply migrations for specified context
    /// <para>will be ignored if environment variable DONT_RUN_MIGRATIONS = true</para>
    /// </summary>
    public static async Task MigrateContextAsync<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        if (Environment.GetEnvironmentVariable("DONT_RUN_MIGRATIONS")?.Any() == true)
        {
            app.ApplicationServices
                .GetRequiredService<ILogger<TContext>>()
                .LogWarning("Migrations of {Context} skipped due to 'DONT_RUN_MIGRATIONS' environment variable", typeof(TContext));

            return;
        }

        var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            await context.Database.MigrateAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            logger.LogError(ex, "An error occurred");
            Environment.Exit(-1);
        }
    }
}
