namespace TodoListApp.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
}

