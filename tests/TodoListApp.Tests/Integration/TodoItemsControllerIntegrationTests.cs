using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TodoListApp.Application.Features.Auth.Commands;
using TodoListApp.Application.Features.Auth.Dtos;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Domain.TodoItems.ValueObjects;
using TodoListApp.Infrastructure.Persistence;

namespace TodoListApp.Tests.Integration;

public class TodoItemsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TodoItemsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Ensure database is created
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateTodoItem_WithValidData_ShouldReturnCreatedTodoItem()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createTodoItemCommand = new CreateTodoItemCommand(
            "Integration Test Todo Item",
            "Test Description",
            DateTime.UtcNow.AddDays(7),
            PriorityLevel.High);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/todoitems", createTodoItemCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItemDto = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        todoItemDto.Should().NotBeNull();
        todoItemDto!.Name.Should().Be(createTodoItemCommand.Name);
        todoItemDto.Description.Should().Be(createTodoItemCommand.Description);
        todoItemDto.Priority.Should().Be(createTodoItemCommand.Priority);
        todoItemDto.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateTodoItem_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var createTodoItemCommand = new CreateTodoItemCommand(
            "Test Todo Item",
            "Description",
            DateTime.UtcNow.AddDays(7));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/todoitems", createTodoItemCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTodoItems_WithAuthentication_ShouldReturnPagedResults()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/todoitems?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNull();
        
        // The response should contain pagination structure
        content.Should().Contain("items");
        content.Should().Contain("totalCount");
        content.Should().Contain("page");
        content.Should().Contain("pageSize");
    }

    private async Task<string> GetAuthTokenAsync()
    {
        // First register a user
        var registerCommand = new RegisterCommand
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Password = "Password123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerCommand);
        
        if (!registerResponse.IsSuccessStatusCode)
        {
            // If registration fails (user might already exist), try to login
            var loginCommand = new LoginCommand(registerCommand.Email, registerCommand.Password);
            var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginCommand);
            
            if (loginResponse.IsSuccessStatusCode)
            {
                var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultDto>();
                return loginResult!.AccessToken;
            }
            
            throw new InvalidOperationException("Could not authenticate test user");
        }

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        return registerResult!.AccessToken;
    }
}
