using FluentAssertions;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Tests.Domain.TodoItems;

public class TodoItemTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_ShouldCreateTodoItemWithCorrectProperties()
    {
        // Arrange
        var name = TodoItemName.Create("Test Todo Item");
        var description = TodoItemDescription.Create("Test Description");
        var dueDate = DueDate.Create(_now.AddDays(7));
        var priority = TodoItemPriority.High;

        // Act
        var todoItem = TodoItem.Create(_userId, name, description, dueDate, priority, _now);

        // Assert
        todoItem.Should().NotBeNull();
        todoItem.Id.Should().NotBeEmpty();
        todoItem.UserId.Should().Be(_userId);
        todoItem.Name.Should().Be(name);
        todoItem.Description.Should().Be(description);
        todoItem.DueDate.Should().Be(dueDate);
        todoItem.Priority.Should().Be(priority);
        todoItem.IsCompleted.Should().BeFalse();
        todoItem.CompletedAtUtc.Should().BeNull();
        todoItem.IsDeleted.Should().BeFalse();
        todoItem.DeletedAtUtc.Should().BeNull();
        todoItem.CreatedAt.Should().BeCloseTo(_now, TimeSpan.FromSeconds(1));
        todoItem.UpdatedAt.Should().BeCloseTo(_now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ToggleComplete_WhenNotCompleted_ShouldMarkAsCompleted()
    {
        // Arrange
        var todoItem = CreateTestTodoItem();
        var completionTime = _now.AddMinutes(30);

        // Act
        todoItem.ToggleComplete(completionTime);

        // Assert
        todoItem.IsCompleted.Should().BeTrue();
        todoItem.CompletedAtUtc.Should().Be(completionTime);
        todoItem.UpdatedAt.Should().Be(completionTime);
    }

    [Fact]
    public void ToggleComplete_WhenCompleted_ShouldMarkAsNotCompleted()
    {
        // Arrange
        var todoItem = CreateTestTodoItem();
        todoItem.ToggleComplete(_now.AddMinutes(30));
        var uncompleteTime = _now.AddHours(1);

        // Act
        todoItem.ToggleComplete(uncompleteTime);

        // Assert
        todoItem.IsCompleted.Should().BeFalse();
        todoItem.CompletedAtUtc.Should().BeNull();
        todoItem.UpdatedAt.Should().Be(uncompleteTime);
    }

    [Fact]
    public void Update_ShouldUpdateTodoItemProperties()
    {
        // Arrange
        var todoItem = CreateTestTodoItem();
        var newName = TodoItemName.Create("Updated Todo Item");
        var newDescription = TodoItemDescription.Create("Updated Description");
        var newDueDate = DueDate.Create(_now.AddDays(14));
        var newPriority = TodoItemPriority.Critical;
        var updateTime = _now.AddHours(1);

        // Act
        todoItem.Update(newName, newDescription, newDueDate, newPriority, updateTime);

        // Assert
        todoItem.Name.Should().Be(newName);
        todoItem.Description.Should().Be(newDescription);
        todoItem.DueDate.Should().Be(newDueDate);
        todoItem.Priority.Should().Be(newPriority);
        todoItem.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void Delete_ShouldMarkTodoItemAsDeleted()
    {
        // Arrange
        var todoItem = CreateTestTodoItem();

        // Act
        todoItem.Delete();

        // Assert
        todoItem.IsDeleted.Should().BeTrue();
        todoItem.DeletedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Restore_ShouldMarkTodoItemAsNotDeleted()
    {
        // Arrange
        var todoItem = CreateTestTodoItem();
        todoItem.Delete();

        // Act
        todoItem.Restore();

        // Assert
        todoItem.IsDeleted.Should().BeFalse();
        todoItem.DeletedAtUtc.Should().BeNull();
    }

    private TodoItem CreateTestTodoItem()
    {
        return TodoItem.Create(
            _userId,
            TodoItemName.Create("Test Todo Item"),
            TodoItemDescription.Create("Test Description"),
            DueDate.Create(_now.AddDays(7)),
            TodoItemPriority.Medium,
            _now);
    }
}
