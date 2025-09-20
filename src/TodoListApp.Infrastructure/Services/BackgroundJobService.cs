using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.Application.Abstractions;
using TodoListApp.Infrastructure.Persistence;

namespace TodoListApp.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<BackgroundJobService> _logger;

    public BackgroundJobService(AppDbContext context, INotificationService notificationService, ILogger<BackgroundJobService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task CleanupExpiredRefreshTokensAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting cleanup of expired refresh tokens");

        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresUtc < DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Cleaned up {Count} expired refresh tokens", expiredTokens.Count);
        }
    }

    public async Task SendOverdueRemindersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking for overdue todo items");

        var overdueTodos = await _context.TodoItems
            .Where(t => !t.IsCompleted && 
                       t.DueDate.Value.HasValue && 
                       t.DueDate.Value < DateTime.UtcNow)
            .GroupBy(t => t.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        foreach (var userOverdue in overdueTodos)
        {
            await _notificationService.NotifyOverdueTodoItemsAsync(userOverdue.UserId, userOverdue.Count, cancellationToken);
        }

        _logger.LogInformation("Sent overdue reminders to {UserCount} users", overdueTodos.Count);
    }

    public async Task GenerateDailyReportAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating daily reports");

        var yesterday = DateTime.UtcNow.AddDays(-1);
        var users = await _context.Users.ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            var stats = await GetUserStatsAsync(user.Id, yesterday, DateTime.UtcNow, cancellationToken);
            // In a real app, you'd send this via email or save to a reports table
            _logger.LogInformation("Daily report for user {UserId}: {Stats}", user.Id, stats);
        }
    }

    public async Task GenerateWeeklyReportAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating weekly reports");

        var lastWeek = DateTime.UtcNow.AddDays(-7);
        var users = await _context.Users.ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            var stats = await GetUserStatsAsync(user.Id, lastWeek, DateTime.UtcNow, cancellationToken);
            // In a real app, you'd send this via email or save to a reports table
            _logger.LogInformation("Weekly report for user {UserId}: {Stats}", user.Id, stats);
        }
    }

    private async Task<object> GetUserStatsAsync(Guid userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var totalTodos = await _context.TodoItems
            .Where(t => t.UserId == userId && t.CreatedAt >= fromDate && t.CreatedAt <= toDate)
            .CountAsync(cancellationToken);

        var completedTodos = await _context.TodoItems
            .Where(t => t.UserId == userId && t.IsCompleted && 
                       t.CompletedAtUtc.HasValue && t.CompletedAtUtc >= fromDate && t.CompletedAtUtc <= toDate)
            .CountAsync(cancellationToken);

        var overdueTodos = await _context.TodoItems
            .Where(t => t.UserId == userId && !t.IsCompleted && 
                       t.DueDate.Value.HasValue && t.DueDate.Value < DateTime.UtcNow)
            .CountAsync(cancellationToken);

        return new
        {
            TotalCreated = totalTodos,
            Completed = completedTodos,
            Overdue = overdueTodos,
            CompletionRate = totalTodos > 0 ? (double)completedTodos / totalTodos * 100 : 0
        };
    }
}
