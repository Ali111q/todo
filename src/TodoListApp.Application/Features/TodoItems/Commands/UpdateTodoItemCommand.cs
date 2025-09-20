using MediatR;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Application.Features.TodoItems.Commands;

public sealed record UpdateTodoItemCommand(string Name, string? Description, DateTime? DueDate, PriorityLevel Priority = PriorityLevel.Medium) : IRequest<TodoItemDto>;

public sealed record UpdateTodoItemCommandWithId(Guid Id, string Name, string? Description, DateTime? DueDate, PriorityLevel Priority = PriorityLevel.Medium) : IRequest<TodoItemDto>;
