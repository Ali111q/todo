using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.Auth.Commands;
using TodoListApp.Application.Features.Auth.Dtos;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Features.Auth;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime)
    {
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
    }

    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenService.GetActiveRefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        if (refreshToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = refreshToken.User;
        
        // Generate new tokens
        var accessToken = _jwtTokenService.CreateToken(user, _dateTime.UtcNow);
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
        
        // Revoke old refresh token and replace with new one
        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, newRefreshToken.Token, cancellationToken);
        
        // Add new refresh token
        user.AddRefreshToken(newRefreshToken);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        
        var userDto = new UserDto(user.Id, user.UserName!, user.Email!);
        
        return new AuthResultDto(accessToken.AccessToken, newRefreshToken.Token, accessToken.ExpiresAtUtc, userDto);
    }
}
