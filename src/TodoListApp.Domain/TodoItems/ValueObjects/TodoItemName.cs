namespace TodoListApp.Domain.TodoItems.ValueObjects;

public sealed record TodoItemName
{
    private TodoItemName(string value) => Value = value;
    public string Value { get; }
    public static TodoItemName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Todo item name is required");
        if (value.Length > 120) throw new ArgumentException("Todo item name max length is 120");
        return new TodoItemName(value.Trim());
    }
    public override string ToString() => Value;
}
