using TodoListApp.Domain.Abstractions;

namespace TodoListApp.Domain.TodoItems.Events;

public sealed record TodoItemCreatedDomainEvent(Guid UserId, Guid TodoItemId, string TodoItemName, DateTime OccurredOnUtc) : DomainEvent(OccurredOnUtc);
