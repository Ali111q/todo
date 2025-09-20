using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.Auth.Commands;
using TodoListApp.Application.Features.Auth.Dtos;
using TodoListApp.Application.Common.Exceptions;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Features.Auth;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
    private readonly IDateTime _dateTime;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IDateTime dateTime)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _dateTime = dateTime;
    }

    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if username or email already exists
            var existingUserByName = await _userManager.FindByNameAsync(request.Username);
            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingUserByName != null || existingUserByEmail != null)
            {
                var conflicts = new List<string>();

                if (existingUserByName != null)
                {
                    conflicts.Add("username");
                }

                if (existingUserByEmail != null)
                {
                    conflicts.Add("email");
                }

                var conflictMessage = conflicts.Count == 1
                    ? $"This {conflicts[0]} is already taken"
                    : $"This {string.Join(" and ", conflicts)} are already taken";

                throw new DuplicateRegistrationException($"Registration failed: {conflictMessage}. Please choose different credentials.");
            }

            var user = new User(request.Username, request.Email);
            
            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            // Assign default role
            if (await _roleManager.RoleExistsAsync(Role.Names.User))
            {
                await _userManager.AddToRoleAsync(user, Role.Names.User);
            }

            // Create tokens
            var accessToken = _jwtTokenService.CreateToken(user, _dateTime.UtcNow);
            var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
            
            user.AddRefreshToken(refreshToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            var userDto = new UserDto(user.Id, user.UserName!, user.Email!);
            
            return new AuthResultDto(accessToken.AccessToken, refreshToken.Token, accessToken.ExpiresAtUtc, userDto);
        }
        catch (DuplicateRegistrationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Registration failed due to an unexpected error. Please try again later.");
        }
    }
}
