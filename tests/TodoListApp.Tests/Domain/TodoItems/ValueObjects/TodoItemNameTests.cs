using FluentAssertions;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Tests.Domain.TodoItems.ValueObjects;

public class TodoItemNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldReturnTodoItemName()
    {
        // Arrange
        var name = "Valid Todo Item Name";

        // Act
        var todoItemName = TodoItemName.Create(name);

        // Assert
        todoItemName.Should().NotBeNull();
        todoItemName.Value.Should().Be(name);
    }

    [Fact]
    public void Create_WithWhitespaces_ShouldTrimName()
    {
        // Arrange
        var name = "  Todo Item Name  ";

        // Act
        var todoItemName = TodoItemName.Create(name);

        // Assert
        todoItemName.Value.Should().Be("Todo Item Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        Action act = () => TodoItemName.Create(invalidName);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Todo item name is required");
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrowArgumentException()
    {
        // Arrange
        var longName = new string('a', 121);

        // Act & Assert
        Action act = () => TodoItemName.Create(longName);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Todo item name max length is 120");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var name = "Test Todo Item";
        var todoItemName = TodoItemName.Create(name);

        // Act
        var result = todoItemName.ToString();

        // Assert
        result.Should().Be(name);
    }
}
