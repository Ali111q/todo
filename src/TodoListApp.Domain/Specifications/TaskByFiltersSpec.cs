using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Domain.Specifications;

public sealed class TodoItemByFiltersSpec : Specification<TodoItem>
{
    public TodoItemByFiltersSpec(Guid userId, bool? completed, DateTime? dueOnOrBefore, int? skip, int? take, string sortBy = "CreatedAt", bool sortDescending = true)
    {
        Criteria = t =>
            t.UserId == userId
            && (!completed.HasValue || t.IsCompleted == completed)
            && (!dueOnOrBefore.HasValue || (t.DueDate.Value.HasValue && t.DueDate.Value <= dueOnOrBefore));

        // Apply sorting based on the sortBy parameter
        switch (sortBy.ToLowerInvariant())
        {
            case "name":
                ApplyOrderBy(t => t.Name.Value, desc: sortDescending);
                break;
            case "duedate":
                ApplyOrderBy(t => t.DueDate.Value ?? DateTime.MaxValue, desc: sortDescending);
                break;
            case "priority":
                ApplyOrderBy(t => t.Priority.Value, desc: sortDescending);
                break;
            case "completed":
                ApplyOrderBy(t => t.IsCompleted, desc: sortDescending);
                break;
            case "createdat":
            default:
                ApplyOrderBy(t => t.CreatedAt, desc: sortDescending);
                break;
        }

        if (skip.HasValue && take.HasValue) ApplyPaging(skip.Value, take.Value);
    }
}

