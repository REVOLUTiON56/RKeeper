using RKeeper.Identity.Data;
using RKeeper.Infrastructure.Extensions;

namespace RKeeper.Identity;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.Configure();
        app.SetupGlobalExceptionLogging();

        await app.MigrateContextAsync<RKeeperIdentityDbContext>();

        await app.RunAsync().ConfigureAwait(false);
    }
}
