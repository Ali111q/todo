using FluentAssertions;
using FluentValidation.TestHelper;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Validation.TodoItems;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Tests.Application.TodoItems.Validation;

public class CreateTodoItemCommandValidatorTests
{
    private readonly CreateTodoItemCommandValidator _validator;

    public CreateTodoItemCommandValidatorTests()
    {
        _validator = new CreateTodoItemCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateTodoItemCommand(
            "Valid Todo Item Name",
            "Valid Description",
            DateTime.UtcNow.AddDays(7),
            PriorityLevel.Medium);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithInvalidName_ShouldHaveValidationError(string invalidName)
    {
        // Arrange
        var command = new CreateTodoItemCommand(invalidName, "Description", DateTime.UtcNow.AddDays(7));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('a', 121);
        var command = new CreateTodoItemCommand(longName, "Description", DateTime.UtcNow.AddDays(7));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithTooLongDescription_ShouldHaveValidationError()
    {
        // Arrange
        var longDescription = new string('a', 1001);
        var command = new CreateTodoItemCommand("Valid Name", longDescription, DateTime.UtcNow.AddDays(7));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateTodoItemCommand("Valid Name", null, DateTime.UtcNow.AddDays(7));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithInvalidPriority_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTodoItemCommand("Valid Name", "Description", DateTime.UtcNow.AddDays(7), (PriorityLevel)999);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }
}
