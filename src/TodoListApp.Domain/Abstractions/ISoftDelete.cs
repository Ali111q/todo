namespace TodoListApp.Domain.Abstractions;

public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }
    void Delete();
    void Restore();
}
