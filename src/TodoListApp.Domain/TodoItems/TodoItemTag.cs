namespace TodoListApp.Domain.TodoItems;

public sealed class TodoItemTag
{
    public Guid TodoItemId { get; set; }
    public Guid TagId { get; set; }

    // Navigation properties
    public TodoItem TodoItem { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
