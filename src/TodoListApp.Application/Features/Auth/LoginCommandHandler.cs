using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.Auth.Commands;
using TodoListApp.Application.Features.Auth.Dtos;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Features.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
    private readonly IDateTime _clock;

    public LoginCommandHandler(
        UserManager<User> userManager, 
        IJwtTokenService jwt, 
        IRefreshTokenService refreshTokenService,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IDateTime clock)
    {
        _userManager = userManager;
        _jwt = jwt;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _clock = clock;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail) 
                   ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Create access token
        var accessToken = _jwt.CreateToken(user, _clock.UtcNow);
        
        // Create refresh token
        var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
        user.AddRefreshToken(refreshToken);
        
        await _refreshTokenRepository.AddAsync(refreshToken, ct);

        var userDto = new UserDto(user.Id, user.UserName!, user.Email!);
        
        return new AuthResultDto(accessToken.AccessToken, refreshToken.Token, accessToken.ExpiresAtUtc, userDto);
    }
}
