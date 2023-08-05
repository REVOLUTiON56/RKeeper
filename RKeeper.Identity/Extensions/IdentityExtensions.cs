using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RKeeper.Identity.Data;
using RKeeper.Identity.Models;

namespace RKeeper.Identity.Extensions;

public static class IdentityExtensions
{
    public static void AddRKeeperIdentityContext(this WebApplicationBuilder builder) {
        var connectionString = builder.Configuration.GetConnectionString(RKeeperIdentityDbContext.ConfigurationName) ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<RKeeperIdentityDbContext>(options => {
            options.UseNpgsql(connectionString);
        });
    }

    public static IdentityBuilder AddRKeeperIdentity(this WebApplicationBuilder builder, Action<IdentityOptions>? configureIdentityOptions = null)
    {
        return builder.Services.AddRKeeperIdentity(configureIdentityOptions);
    }

    public static IdentityBuilder AddRKeeperIdentity(this IServiceCollection services, Action<IdentityOptions>? configureIdentityOptions = null)
    {
        var identityBuilder = services
            .AddIdentityCore<ApplicationUser>(i =>
            {
                i.Password.RequireUppercase = false;
                i.Password.RequireDigit = false;
                i.Password.RequireNonAlphanumeric = false;
                i.Password.RequiredLength = 6;
                i.SignIn.RequireConfirmedAccount = false;
                i.SignIn.RequireConfirmedEmail = false;
                i.SignIn.RequireConfirmedPhoneNumber = false;
                configureIdentityOptions?.Invoke(i);
            })
            .AddEntityFrameworkStores<RKeeperIdentityDbContext>()
            .AddRoles<ApplicationRole>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddSignInManager<SignInManager<ApplicationUser>>();

        return identityBuilder;
    }
}
