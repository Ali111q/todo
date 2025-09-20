using MediatR;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.Auth.Commands;

namespace TodoListApp.Application.Features.Auth;

public sealed class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RevokeTokenCommandHandler(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenService.GetActiveRefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        if (refreshToken != null)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, null, cancellationToken);
        }
    }
}
