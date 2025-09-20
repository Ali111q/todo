using Microsoft.Extensions.Logging;
using TodoListApp.Application.Abstractions;

namespace TodoListApp.Infrastructure.Services;

public class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;

    public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task NotifyTodoItemCreatedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Todo item created: {TodoItemName} for user {UserId}", todoItemName, userId);
        await Task.CompletedTask;
    }

    public async Task NotifyTodoItemUpdatedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Todo item updated: {TodoItemName} for user {UserId}", todoItemName, userId);
        await Task.CompletedTask;
    }

    public async Task NotifyTodoItemCompletedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Todo item completed: {TodoItemName} for user {UserId}", todoItemName, userId);
        await Task.CompletedTask;
    }

    public async Task NotifyTodoItemDeletedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Todo item deleted: {TodoItemName} for user {UserId}", todoItemName, userId);
        await Task.CompletedTask;
    }

    public async Task NotifyOverdueTodoItemsAsync(Guid userId, int overdueCount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} has {OverdueCount} overdue todo items", userId, overdueCount);
        await Task.CompletedTask;
    }
}
