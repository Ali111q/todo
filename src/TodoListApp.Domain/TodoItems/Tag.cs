using TodoListApp.Domain.Abstractions;

namespace TodoListApp.Domain.TodoItems;

public sealed class Tag : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    // Navigation properties
    public ICollection<TodoItemTag> TodoItemTags { get; private set; } = new List<TodoItemTag>();

    private Tag() { } // EF Core

    private Tag(string name, string color, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Color = color;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Tag Create(string name, string color, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name is required", nameof(name));
        if (name.Length > 50)
            throw new ArgumentException("Tag name cannot exceed 50 characters", nameof(name));

        return new Tag(name.Trim(), color?.Trim() ?? "#007bff", userId);
    }

    public void Update(string name, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name is required", nameof(name));
        if (name.Length > 50)
            throw new ArgumentException("Tag name cannot exceed 50 characters", nameof(name));

        Name = name.Trim();
        Color = color?.Trim() ?? "#007bff";
        UpdatedAt = DateTime.UtcNow;
    }
}
