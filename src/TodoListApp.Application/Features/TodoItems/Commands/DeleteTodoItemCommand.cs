using MediatR;

namespace TodoListApp.Application.Features.TodoItems.Commands;

public sealed record DeleteTodoItemCommand(Guid Id) : IRequest;
