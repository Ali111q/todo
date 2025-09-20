using TodoListApp.Domain.Abstractions;

namespace TodoListApp.Domain.TodoItems.Events;

public sealed record TodoItemCompletionToggledDomainEvent(Guid TodoItemId, bool IsCompleted, DateTime OccurredOnUtc) : DomainEvent(OccurredOnUtc);
