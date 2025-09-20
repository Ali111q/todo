namespace TodoListApp.Domain.TodoItems.ValueObjects;

public sealed record TodoItemPriority
{
    public static readonly TodoItemPriority Low = new(PriorityLevel.Low);
    public static readonly TodoItemPriority Medium = new(PriorityLevel.Medium);
    public static readonly TodoItemPriority High = new(PriorityLevel.High);
    public static readonly TodoItemPriority Critical = new(PriorityLevel.Critical);

    public PriorityLevel Value { get; }

    private TodoItemPriority(PriorityLevel value)
    {
        Value = value;
    }

    public static TodoItemPriority Create(PriorityLevel priority)
    {
        return priority switch
        {
            PriorityLevel.Low => Low,
            PriorityLevel.Medium => Medium,
            PriorityLevel.High => High,
            PriorityLevel.Critical => Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
        };
    }

    public static implicit operator PriorityLevel(TodoItemPriority priority) => priority.Value;
    public static implicit operator TodoItemPriority(PriorityLevel priority) => Create(priority);
    public override string ToString() => Value.ToString();
}

public enum PriorityLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
