using MediatR;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IRepository<TodoItem> _todoItemRepo;
    private readonly ICurrentUser _current;

    public DeleteTodoItemCommandHandler(IRepository<TodoItem> todoItemRepo, ICurrentUser current)
    {
        _todoItemRepo = todoItemRepo;
        _current = current;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken ct)
    {
        var todoItem = await _todoItemRepo.GetByIdAsync(request.Id, ct)
                   ?? throw new KeyNotFoundException("Todo item not found");

        if (todoItem.UserId != _current.UserId)
            throw new UnauthorizedAccessException();

        todoItem.Delete();
        await _todoItemRepo.SaveChangesAsync(ct);
    }
}
