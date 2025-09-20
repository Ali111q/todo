using TodoListApp.Domain.Abstractions;

namespace TodoListApp.Domain.Users;

public sealed class RefreshToken : AggregateRoot
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedUtc { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private RefreshToken() { } // EF Core

    public RefreshToken(string token, DateTime expiresUtc, Guid userId)
    {
        Id = Guid.NewGuid();
        Token = token;
        ExpiresUtc = expiresUtc;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke(string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedUtc = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresUtc;
    public bool IsActive => !IsRevoked && !IsExpired;

    // For testing purposes
    public void SetUser(User user) => User = user;
}
