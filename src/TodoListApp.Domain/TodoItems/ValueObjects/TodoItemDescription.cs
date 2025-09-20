namespace TodoListApp.Domain.TodoItems.ValueObjects;

public sealed record TodoItemDescription
{
    private TodoItemDescription(string? value) => Value = value;
    public string? Value { get; }
    public static TodoItemDescription Create(string? value)
    {
        if (value != null && value.Length > 1000) 
            throw new ArgumentException("Todo item description max length is 1000");
        return new TodoItemDescription(value?.Trim());
    }
    public override string ToString() => Value ?? string.Empty;
}
