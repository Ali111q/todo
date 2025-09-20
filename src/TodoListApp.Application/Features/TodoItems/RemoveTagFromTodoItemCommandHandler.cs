using MediatR;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class RemoveTagFromTodoItemCommandHandler : IRequestHandler<RemoveTagFromTodoItemCommand>
{
    private readonly IRepository<TodoItem> _todoItemRepo;
    private readonly ICurrentUser _currentUser;

    public RemoveTagFromTodoItemCommandHandler(IRepository<TodoItem> todoItemRepo, ICurrentUser currentUser)
    {
        _todoItemRepo = todoItemRepo;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveTagFromTodoItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        
        var todoItem = await _todoItemRepo.GetByIdAsync(request.TodoItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Todo item not found");
            
        if (todoItem.UserId != userId)
            throw new UnauthorizedAccessException();

        todoItem.RemoveTag(request.TagId);
        await _todoItemRepo.SaveChangesAsync(cancellationToken);
    }
}
