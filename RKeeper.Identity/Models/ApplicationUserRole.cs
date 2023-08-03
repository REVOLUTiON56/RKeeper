using Microsoft.AspNetCore.Identity;

namespace RKeeper.Identity.Models;

public class ApplicationUserRole : IdentityUserRole<int>
{
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationRole Role { get; set; } = null!;
}
