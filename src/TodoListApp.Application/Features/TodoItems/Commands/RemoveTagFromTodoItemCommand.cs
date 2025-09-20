using MediatR;

namespace TodoListApp.Application.Features.TodoItems.Commands;

public sealed record RemoveTagFromTodoItemCommand(Guid TodoItemId, Guid TagId) : IRequest;
