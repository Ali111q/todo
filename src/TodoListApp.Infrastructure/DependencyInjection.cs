using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using TodoListApp.Application.Abstractions;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.Users;
using TodoListApp.Infrastructure.Auth;
using TodoListApp.Infrastructure.Persistence;
using TodoListApp.Infrastructure.Services;

namespace TodoListApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(conn);
        });

        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IRepository<TodoItem>, EfRepository<TodoItem>>();
        services.AddScoped<IRepository<RefreshToken>, EfRepository<RefreshToken>>();
        services.AddScoped<IRepository<Tag>, EfRepository<Tag>>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.Configure<JwtOptions>(options => config.GetSection(JwtOptions.SectionName).Bind(options));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IDateTime, SystemDateTime>();
        services.AddScoped<INotificationService, LoggingNotificationService>();
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();

        // HTTP Client Factory
        services.AddHttpClient<IIpLocationService, IpLocationService>(client =>
        {
            client.BaseAddress = new Uri("http://ip-api.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
