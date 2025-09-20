using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.TodoItems.ValueObjects;
using TodoListApp.Domain.Users;

namespace TodoListApp.Tests.Application.TodoItems;

public class CreateTodoItemCommandHandlerTests
{
    private readonly Mock<IRepository<TodoItem>> _mockRepository;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IDateTime> _mockDateTime;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly CreateTodoItemCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateTime _now = DateTime.UtcNow;

    public CreateTodoItemCommandHandlerTests()
    {
        _mockRepository = new Mock<IRepository<TodoItem>>();
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockDateTime = new Mock<IDateTime>();
        _mockNotificationService = new Mock<INotificationService>();
        
        _handler = new CreateTodoItemCommandHandler(
            _mockRepository.Object,
            _mockUserManager.Object,
            _mockCurrentUser.Object,
            _mockDateTime.Object,
            _mockNotificationService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTodoItem()
    {
        // Arrange
        var command = new CreateTodoItemCommand(
            "Test Todo Item",
            "Test Description",
            _now.AddDays(7),
            PriorityLevel.High);

        _mockCurrentUser.Setup(x => x.UserId).Returns(_userId);
        _mockDateTime.Setup(x => x.UtcNow).Returns(_now);
        _mockUserManager.Setup(x => x.FindByIdAsync(_userId.ToString()))
            .ReturnsAsync(new User("testuser", "test@example.com") { Id = _userId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.DueDate.Should().Be(command.DueDate);
        result.Priority.Should().Be(command.Priority);
        result.IsCompleted.Should().BeFalse();
        result.UserId.Should().Be(_userId);

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new CreateTodoItemCommand("Test Todo Item", "Description", _now.AddDays(7));
        _mockCurrentUser.Setup(x => x.UserId).Returns((Guid?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
