using MediatR;

namespace TodoListApp.Application.Features.TodoItems.Commands;

public sealed record AddTagToTodoItemCommand(Guid TodoItemId, Guid TagId) : IRequest;
