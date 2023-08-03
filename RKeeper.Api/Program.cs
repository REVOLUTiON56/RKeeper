using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using RKeeper.Infrastructure.Extensions;

namespace RKeeper.Api;

public class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.Configure();
        app.SetupGlobalExceptionLogging();

        return app.RunAsync();
    }
}
