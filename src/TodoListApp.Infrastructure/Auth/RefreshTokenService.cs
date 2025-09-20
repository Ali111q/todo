using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Application.Abstractions;
using TodoListApp.Domain.Users;
using TodoListApp.Infrastructure.Persistence;

namespace TodoListApp.Infrastructure.Auth;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly IDateTime _dateTime;

    public RefreshTokenService(AppDbContext context, IDateTime dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var token = Convert.ToBase64String(randomBytes);
        var expiresUtc = _dateTime.UtcNow.AddDays(7); // 7 days expiry

        return new RefreshToken(token, expiresUtc, userId);
    }

    public async Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresUtc > _dateTime.UtcNow, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? replacedByToken = null, CancellationToken cancellationToken = default)
    {
        refreshToken.Revoke(replacedByToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user != null)
        {
            user.RevokeAllRefreshTokens();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
