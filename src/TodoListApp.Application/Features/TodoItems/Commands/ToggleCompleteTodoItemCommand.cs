using MediatR;
using TodoListApp.Application.Features.TodoItems.Dtos;

namespace TodoListApp.Application.Features.TodoItems.Commands;

public sealed record ToggleCompleteTodoItemCommand(Guid Id) : IRequest<TodoItemDto>;
