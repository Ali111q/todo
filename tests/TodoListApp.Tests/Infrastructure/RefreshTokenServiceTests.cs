using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoListApp.Application.Abstractions;
using TodoListApp.Domain.Users;
using TodoListApp.Infrastructure.Auth;
using TodoListApp.Infrastructure.Persistence;

namespace TodoListApp.Tests.Infrastructure;

public class RefreshTokenServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IDateTime> _mockDateTime;
    private readonly RefreshTokenService _service;
    private readonly DateTime _now = DateTime.UtcNow;

    public RefreshTokenServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockDateTime = new Mock<IDateTime>();
        _mockDateTime.Setup(x => x.UtcNow).Returns(_now);
        
        _service = new RefreshTokenService(_context, _mockDateTime.Object);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var refreshToken = _service.GenerateRefreshToken(userId);

        // Assert
        refreshToken.Should().NotBeNull();
        refreshToken.Token.Should().NotBeNullOrEmpty();
        refreshToken.UserId.Should().Be(userId);
        refreshToken.ExpiresUtc.Should().Be(_now.AddDays(7));
        refreshToken.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetActiveRefreshTokenAsync_WithValidToken_ShouldReturnToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", "test@example.com") { Id = userId };
        var refreshToken = _service.GenerateRefreshToken(userId);
        refreshToken.SetUser(user);

        _context.Users.Add(user);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetActiveRefreshTokenAsync(refreshToken.Token);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be(refreshToken.Token);
        result.User.Should().NotBeNull();
        result.User.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetActiveRefreshTokenAsync_WithInvalidToken_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetActiveRefreshTokenAsync("invalid-token");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ShouldRevokeToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", "test@example.com") { Id = userId };
        var refreshToken = _service.GenerateRefreshToken(userId);
        refreshToken.SetUser(user);

        _context.Users.Add(user);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        await _service.RevokeRefreshTokenAsync(refreshToken, "replacement-token");

        // Assert
        refreshToken.IsActive.Should().BeFalse();
        refreshToken.RevokedUtc.Should().BeCloseTo(_now, TimeSpan.FromSeconds(5));
        refreshToken.ReplacedByToken.Should().Be("replacement-token");
    }

    [Fact]
    public async Task RevokeAllUserRefreshTokensAsync_ShouldRevokeAllUserTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", "test@example.com") { Id = userId };
        
        var token1 = _service.GenerateRefreshToken(userId);
        var token2 = _service.GenerateRefreshToken(userId);
        token1.SetUser(user);
        token2.SetUser(user);

        _context.Users.Add(user);
        _context.RefreshTokens.AddRange(token1, token2);
        await _context.SaveChangesAsync();

        // Act
        await _service.RevokeAllUserRefreshTokensAsync(userId);

        // Assert
        var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        tokens.Should().HaveCount(2);
        tokens.Should().OnlyContain(t => t.IsRevoked);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
