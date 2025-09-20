using FluentAssertions;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Tests.Domain.TodoItems.ValueObjects;

public class TodoItemPriorityTests
{
    [Theory]
    [InlineData(PriorityLevel.Low)]
    [InlineData(PriorityLevel.Medium)]
    [InlineData(PriorityLevel.High)]
    [InlineData(PriorityLevel.Critical)]
    public void Create_WithValidPriority_ShouldReturnTodoItemPriority(PriorityLevel level)
    {
        // Act
        var priority = TodoItemPriority.Create(level);

        // Assert
        priority.Should().NotBeNull();
        priority.Value.Should().Be(level);
    }

    [Fact]
    public void Create_WithInvalidPriority_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var invalidLevel = (PriorityLevel)999;

        // Act & Assert
        Action act = () => TodoItemPriority.Create(invalidLevel);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        // Assert
        TodoItemPriority.Low.Value.Should().Be(PriorityLevel.Low);
        TodoItemPriority.Medium.Value.Should().Be(PriorityLevel.Medium);
        TodoItemPriority.High.Value.Should().Be(PriorityLevel.High);
        TodoItemPriority.Critical.Value.Should().Be(PriorityLevel.Critical);
    }

    [Fact]
    public void ImplicitConversion_FromPriorityLevel_ShouldWork()
    {
        // Act
        TodoItemPriority priority = PriorityLevel.High;

        // Assert
        priority.Value.Should().Be(PriorityLevel.High);
    }

    [Fact]
    public void ImplicitConversion_ToPriorityLevel_ShouldWork()
    {
        // Arrange
        var priority = TodoItemPriority.Critical;

        // Act
        PriorityLevel level = priority;

        // Assert
        level.Should().Be(PriorityLevel.Critical);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        var priority = TodoItemPriority.High;

        // Act
        var result = priority.ToString();

        // Assert
        result.Should().Be("High");
    }
}
