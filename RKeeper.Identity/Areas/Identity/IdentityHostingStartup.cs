[assembly: HostingStartup(typeof(RKeeper.Identity.Areas.Identity.IdentityHostingStartup))]
namespace RKeeper.Identity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
