using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Application.Features.TodoItems.Dtos;

public sealed record TodoItemDto(
    Guid Id,
    Guid UserId,
    string Username,
    string Name,
    string? Description,
    DateTime? DueDate,
    PriorityLevel Priority,
    bool IsCompleted,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc);
