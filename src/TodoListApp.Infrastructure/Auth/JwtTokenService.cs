using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.Auth.Dtos;
using TodoListApp.Domain.Users;

namespace TodoListApp.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opts;
    private readonly UserManager<User> _userManager;
    
    public JwtTokenService(IOptions<JwtOptions> opts, UserManager<User> userManager)
    {
        _opts = opts.Value;
        _userManager = userManager;
    }

    public TokenResultDto CreateToken(User user, DateTime nowUtc)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Primary claim for user ID
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty), // Username claim
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // Email claim
            new Claim("sub", user.Id.ToString()), // Subject claim (alternative)
            new Claim("user_id", user.Id.ToString()), // Custom claim for user ID
            new Claim("username", user.UserName ?? string.Empty) // Custom claim for username
        };

        // Add role claims
        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expires = nowUtc.AddMinutes(_opts.ExpiryMinutes);
        var token = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expires,
            signingCredentials: creds);

        var raw = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResultDto(raw, expires);
    }
}
