using Microsoft.AspNetCore.SignalR;
using TodoListApp.Application.Abstractions;
using TodoListApp.Infrastructure.Hubs;

namespace TodoListApp.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<TodoItemHub> _hubContext;

    public SignalRNotificationService(IHubContext<TodoItemHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyTodoItemCreatedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"User_{userId}")
            .SendAsync("TodoItemCreated", new { Name = todoItemName, Timestamp = DateTime.UtcNow }, cancellationToken);
    }

    public async Task NotifyTodoItemUpdatedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"User_{userId}")
            .SendAsync("TodoItemUpdated", new { Name = todoItemName, Timestamp = DateTime.UtcNow }, cancellationToken);
    }

    public async Task NotifyTodoItemCompletedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"User_{userId}")
            .SendAsync("TodoItemCompleted", new { Name = todoItemName, Timestamp = DateTime.UtcNow }, cancellationToken);
    }

    public async Task NotifyTodoItemDeletedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"User_{userId}")
            .SendAsync("TodoItemDeleted", new { Name = todoItemName, Timestamp = DateTime.UtcNow }, cancellationToken);
    }

    public async Task NotifyOverdueTodoItemsAsync(Guid userId, int overdueCount, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"User_{userId}")
            .SendAsync("OverdueTodoItems", new { Count = overdueCount, Timestamp = DateTime.UtcNow }, cancellationToken);
    }
}
