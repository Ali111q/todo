using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems.Events;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Domain.TodoItems;

public sealed class TodoItem : AggregateRoot, ISoftDelete
{
    private TodoItem() { } // EF

    public Guid UserId { get; private set; }
    public TodoItemName Name { get; private set; } = null!;
    public TodoItemDescription Description { get; private set; } = null!;
    public DueDate DueDate { get; private set; } = null!;
    public TodoItemPriority Priority { get; private set; } = null!;
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private TodoItem(Guid id, Guid userId, TodoItemName name, TodoItemDescription description, DueDate dueDate, TodoItemPriority priority, DateTime nowUtc)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
        IsCompleted = false;

        Raise(new TodoItemCreatedDomainEvent(UserId, Id, Name.Value, nowUtc));
    }

    public static TodoItem Create(Guid userId, TodoItemName name, TodoItemDescription description, DueDate dueDate, TodoItemPriority priority, DateTime nowUtc)
        => new(Guid.NewGuid(), userId, name, description, dueDate, priority, nowUtc);

    public void Update(TodoItemName name, TodoItemDescription description, DueDate dueDate, TodoItemPriority priority, DateTime nowUtc)
    {
        Name = name;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        UpdatedAt = nowUtc;
    }

    public void ToggleComplete(DateTime nowUtc)
    {
        IsCompleted = !IsCompleted;
        CompletedAtUtc = IsCompleted ? nowUtc : null;
        UpdatedAt = nowUtc;
        Raise(new TodoItemCompletionToggledDomainEvent(Id, IsCompleted, nowUtc));
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
