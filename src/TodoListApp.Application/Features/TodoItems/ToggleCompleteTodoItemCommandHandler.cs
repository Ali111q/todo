using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class ToggleCompleteTodoItemCommandHandler : IRequestHandler<ToggleCompleteTodoItemCommand, TodoItemDto>
{
    private readonly IRepository<TodoItem> _todoItemRepo;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _current;
    private readonly IDateTime _clock;
    private readonly INotificationService _notificationService;

    public ToggleCompleteTodoItemCommandHandler(IRepository<TodoItem> todoItemRepo, UserManager<User> userManager, ICurrentUser current, IDateTime clock, INotificationService notificationService)
    {
        _todoItemRepo = todoItemRepo;
        _userManager = userManager;
        _current = current;
        _clock = clock;
        _notificationService = notificationService;
    }

    public async Task<TodoItemDto> Handle(ToggleCompleteTodoItemCommand request, CancellationToken ct)
    {
        var todoItem = await _todoItemRepo.GetByIdAsync(request.Id, ct)
                   ?? throw new KeyNotFoundException("Todo item not found");

        if (todoItem.UserId != _current.UserId)
            throw new UnauthorizedAccessException();

        todoItem.ToggleComplete(_clock.UtcNow);
        await _todoItemRepo.SaveChangesAsync(ct);

        // Send notification if completed
        if (todoItem.IsCompleted)
        {
            await _notificationService.NotifyTodoItemCompletedAsync(todoItem.UserId, todoItem.Name.Value, ct);
        }

        var user = await _userManager.FindByIdAsync(todoItem.UserId.ToString());

        return new TodoItemDto(
            todoItem.Id,
            todoItem.UserId,
            user?.UserName ?? "Unknown User",
            todoItem.Name.Value,
            todoItem.Description.Value,
            todoItem.DueDate.Value,
            todoItem.Priority.Value,
            todoItem.IsCompleted,
            todoItem.CreatedAt,
            todoItem.CompletedAtUtc,
            new List<TagDto>());
    }
}
