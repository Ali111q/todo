using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Abstractions;

public interface IRefreshTokenService
{
    RefreshToken GenerateRefreshToken(Guid userId);
    Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? replacedByToken = null, CancellationToken cancellationToken = default);
    Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
