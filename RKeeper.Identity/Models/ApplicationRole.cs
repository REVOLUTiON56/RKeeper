using Microsoft.AspNetCore.Identity;

namespace RKeeper.Identity.Models;

public interface IRole
{
    /// <summary>
    /// Gets or sets the primary key for this role.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Gets or sets the name for this role.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// Gets or sets the normalized name for this role.
    /// </summary>
    string? NormalizedName { get; set; }

    /// <summary>
    /// A random value that should change whenever a role is persisted to the store
    /// </summary>
    string? ConcurrencyStamp { get; set; }
}

public class ApplicationRole : IdentityRole<int>, IRole
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public List<IUser> Users => UserRoles.Select(x => x.User).Cast<IUser>().ToList();
}
