using Microsoft.AspNetCore.Identity;

namespace TodoListApp.Domain.Users;

public sealed class Role : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Role() : base()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Role(string roleName) : this()
    {
        Name = roleName;
        NormalizedName = roleName.ToUpperInvariant();
    }

    public static class Names
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
