using TodoListApp.Application.Abstractions;

namespace TodoListApp.Infrastructure.Services;

public sealed class SystemDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}

