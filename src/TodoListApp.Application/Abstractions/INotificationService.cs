namespace TodoListApp.Application.Abstractions;

public interface INotificationService
{
    Task NotifyTodoItemCreatedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default);
    Task NotifyTodoItemUpdatedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default);
    Task NotifyTodoItemCompletedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default);
    Task NotifyTodoItemDeletedAsync(Guid userId, string todoItemName, CancellationToken cancellationToken = default);
    Task NotifyOverdueTodoItemsAsync(Guid userId, int overdueCount, CancellationToken cancellationToken = default);
}
