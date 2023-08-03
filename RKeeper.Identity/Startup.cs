using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using RKeeper.Identity.Extensions;
using RKeeper.Identity.Services;

namespace RKeeper.Identity;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddRKeeperIdentityContext();
        builder.AddRKeeperIdentity();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddRazorPages();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Error/";
        });

        builder.Services.AddTransient<IEmailSender, EmailSender>();
    }

    public static void Configure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
    }
}
