using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.TodoItems.ValueObjects;
using TodoListApp.Domain.Users;

namespace TodoListApp.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        // Seed roles
        if (!await roleManager.RoleExistsAsync(Role.Names.Admin))
        {
            await roleManager.CreateAsync(new Role(Role.Names.Admin));
        }
        
        if (!await roleManager.RoleExistsAsync(Role.Names.User))
        {
            await roleManager.CreateAsync(new Role(Role.Names.User));
        }

        // Seed users
        if (await context.Users.AnyAsync())
            return; // Already seeded

        var user1 = new User("john_doe", "john@example.com");
        user1.Id = new Guid("11111111-1111-1111-1111-111111111111");
        
        var result1 = await userManager.CreateAsync(user1, "Password123!");
        if (result1.Succeeded)
        {
            await userManager.AddToRoleAsync(user1, Role.Names.Admin);
        }

        var user2 = new User("jane_smith", "jane@example.com");
        user2.Id = new Guid("22222222-2222-2222-2222-222222222222");
        
        var result2 = await userManager.CreateAsync(user2, "Password123!");
        if (result2.Succeeded)
        {
            await userManager.AddToRoleAsync(user2, Role.Names.User);
        }

        await context.SaveChangesAsync();

        // Seed todo items
        var now = DateTime.UtcNow;
        var todoItems1 = new[]
        {
            TodoItem.Create(user1.Id, TodoItemName.Create("Complete API Documentation"), TodoItemDescription.Create("Write comprehensive API documentation"), DueDate.Create(now.AddDays(7)), TodoItemPriority.High, now),
            TodoItem.Create(user1.Id, TodoItemName.Create("Setup CI/CD Pipeline"), TodoItemDescription.Create("Configure automated deployment pipeline"), DueDate.Create(now.AddDays(14)), TodoItemPriority.Medium, now),
            TodoItem.Create(user1.Id, TodoItemName.Create("Code Review"), TodoItemDescription.Create("Review pull requests from team"), DueDate.Create(now.AddDays(2)), TodoItemPriority.Critical, now)
        };

        var todoItems2 = new[]
        {
            TodoItem.Create(user2.Id, TodoItemName.Create("Design Database Schema"), TodoItemDescription.Create("Create ERD for new features"), DueDate.Create(now.AddDays(5)), TodoItemPriority.High, now),
            TodoItem.Create(user2.Id, TodoItemName.Create("Write Unit Tests"), TodoItemDescription.Create("Add test coverage for core functionality"), DueDate.Create(now.AddDays(10)), TodoItemPriority.Low, now)
        };

        context.TodoItems.AddRange(todoItems1);
        context.TodoItems.AddRange(todoItems2);

        await context.SaveChangesAsync();
    }
}
