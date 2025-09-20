using MediatR;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class AddTagToTodoItemCommandHandler : IRequestHandler<AddTagToTodoItemCommand>
{
    private readonly IRepository<TodoItem> _todoItemRepo;
    private readonly IRepository<Tag> _tagRepo;
    private readonly ICurrentUser _currentUser;

    public AddTagToTodoItemCommandHandler(IRepository<TodoItem> todoItemRepo, IRepository<Tag> tagRepo, ICurrentUser currentUser)
    {
        _todoItemRepo = todoItemRepo;
        _tagRepo = tagRepo;
        _currentUser = currentUser;
    }

    public async Task Handle(AddTagToTodoItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        
        var todoItem = await _todoItemRepo.GetByIdAsync(request.TodoItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Todo item not found");
            
        if (todoItem.UserId != userId)
            throw new UnauthorizedAccessException();
            
        var tag = await _tagRepo.GetByIdAsync(request.TagId, cancellationToken)
            ?? throw new KeyNotFoundException("Tag not found");
            
        if (tag.UserId != userId)
            throw new UnauthorizedAccessException();

        todoItem.AddTag(tag);
        await _todoItemRepo.SaveChangesAsync(cancellationToken);
    }
}
