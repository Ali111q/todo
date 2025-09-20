namespace TodoListApp.Domain.Abstractions;

public abstract record DomainEvent(DateTime OccurredOnUtc);

