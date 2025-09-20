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

public sealed class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommandWithId, TodoItemDto>
{
    private readonly IRepository<TodoItem> _todoItemRepo;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _current;
    private readonly IDateTime _clock;

    public UpdateTodoItemCommandHandler(IRepository<TodoItem> todoItemRepo, UserManager<User> userManager, ICurrentUser current, IDateTime clock)
    {
        _todoItemRepo = todoItemRepo;
        _userManager = userManager;
        _current = current;
        _clock = clock;
    }

    public async Task<TodoItemDto> Handle(UpdateTodoItemCommandWithId request, CancellationToken ct)
    {
        var todoItem = await _todoItemRepo.GetByIdAsync(request.Id, ct)
                   ?? throw new KeyNotFoundException("Todo item not found");

        if (todoItem.UserId != _current.UserId)
            throw new UnauthorizedAccessException();

        var utcDueDate = request.DueDate?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DueDate.Value, DateTimeKind.Utc)
            : request.DueDate?.ToUniversalTime();

        todoItem.Update(TodoItemName.Create(request.Name), TodoItemDescription.Create(request.Description), DueDate.Create(utcDueDate), TodoItemPriority.Create(request.Priority), _clock.UtcNow);
        await _todoItemRepo.SaveChangesAsync(ct);

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
            todoItem.CompletedAtUtc);
    }
}
