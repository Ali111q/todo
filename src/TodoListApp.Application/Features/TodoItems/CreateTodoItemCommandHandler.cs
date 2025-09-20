using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.TodoItems.ValueObjects;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, TodoItemDto>
{
    private readonly IRepository<TodoItem> _repo;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _current;
    private readonly IDateTime _clock;
    private readonly INotificationService _notificationService;

    public CreateTodoItemCommandHandler(IRepository<TodoItem> repo, UserManager<User> userManager, ICurrentUser current, IDateTime clock, INotificationService notificationService)
    { _repo = repo; _userManager = userManager; _current = current; _clock = clock; _notificationService = notificationService; }

    public async Task<TodoItemDto> Handle(CreateTodoItemCommand request, CancellationToken ct)
    {
        var ownerId = _current.UserId ?? throw new UnauthorizedAccessException();
        var utcDueDate = request.DueDate?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DueDate.Value, DateTimeKind.Utc)
            : request.DueDate?.ToUniversalTime();

        var todoItem = TodoItem.Create(ownerId,
            TodoItemName.Create(request.Name),
            TodoItemDescription.Create(request.Description),
            DueDate.Create(utcDueDate),
            TodoItemPriority.Create(request.Priority),
            _clock.UtcNow);

        await _repo.AddAsync(todoItem, ct);
        await _repo.SaveChangesAsync(ct);

        // Send notification
        await _notificationService.NotifyTodoItemCreatedAsync(todoItem.UserId, todoItem.Name.Value, ct);

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
