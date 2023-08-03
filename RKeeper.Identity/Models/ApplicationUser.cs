using Microsoft.AspNetCore.Identity;

namespace RKeeper.Identity.Models;

public interface IUser
{
    /// <summary>
    /// Gets or sets a value when record was created
    /// </summary>
    DateTime CreateDate { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has deleted.
    /// </summary>
    bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the deletion time
    /// </summary>
    DateTime? DeletionDateTime { get; set; }

    /// <summary>
    /// Gets or sets the last login time.
    /// </summary>
    DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// Gets or sets the last activity time.
    /// </summary>
    DateTime? LastActivityTime { get; set; }

    /// <summary>
    /// Gets or sets list of roles.
    /// </summary>
    List<IRole> Roles { get; }

    /// <summary>
    /// Gets or sets the primary key for this user.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Gets or sets the user name for this user.
    /// </summary>
    string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the normalized user name for this user.
    /// </summary>
    string? NormalizedUserName { get; set; }

    /// <summary>
    /// Gets or sets the email address for this user.
    /// </summary>
    string? Email { get; set; }

    /// <summary>
    /// Gets or sets the normalized email address for this user.
    /// </summary>
    string? NormalizedEmail { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their email address.
    /// </summary>
    /// <value>True if the email address has been confirmed, otherwise false.</value>
    bool EmailConfirmed { get; set; }

    /// <summary>
    /// Gets or sets a salted and hashed representation of the password for this user.
    /// </summary>
    string? PasswordHash { get; set; }

    /// <summary>
    /// A random value that must change whenever a users credentials change (password changed, login removed)
    /// </summary>
    string? SecurityStamp { get; set; }

    /// <summary>
    /// A random value that must change whenever a user is persisted to the store
    /// </summary>
    string? ConcurrencyStamp { get; set; }

    /// <summary>
    /// Gets or sets a telephone number for the user.
    /// </summary>
    string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their telephone address.
    /// </summary>
    /// <value>True if the telephone number has been confirmed, otherwise false.</value>
    bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
    /// </summary>
    /// <value>True if 2fa is enabled, otherwise false.</value>
    bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Gets or sets the date and time, in UTC, when any user lockout ends.
    /// </summary>
    /// <remarks>
    /// A value in the past means the user is not locked out.
    /// </remarks>
    DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if the user could be locked out.
    /// </summary>
    /// <value>True if the user could be locked out, otherwise false.</value>
    bool LockoutEnabled { get; set; }

    /// <summary>
    /// Gets or sets the number of failed login attempts for the current user.
    /// </summary>
    int AccessFailedCount { get; set; }
}

public class ApplicationUser : IdentityUser<int>, IUser
{
    public DateTime CreateDate { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletionDateTime { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public DateTime? LastActivityTime { get; set; }
    public List<IRole> Roles => UserRoles.Select(x => x.Role).Cast<IRole>().ToList();
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}
