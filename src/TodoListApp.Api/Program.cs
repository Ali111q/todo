using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TodoListApp.Api.Filters;
using TodoListApp.Application;
using TodoListApp.Infrastructure;
using TodoListApp.Infrastructure.Persistence;
using TodoListApp.Application.Abstractions;
using TodoListApp.Domain.Users;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.AddControllers(o => o.Filters.Add<ApiExceptionFilter>())
                .AddNewtonsoftJson();

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Todo List API", 
        Version = "v1",
        Description = "A comprehensive Todo List API with authentication, role-based authorization, and soft delete support for managing todo items"
    });

    // Configure enums to be displayed as strings
    c.UseAllOfToExtendReferenceSchemas();
    c.UseOneOfForPolymorphism();
    c.SchemaFilter<EnumSchemaFilter>();

    // Include XML comments for better documentation
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "TodoListApp.Api.xml");
    if (File.Exists(xmlFile))
        c.IncludeXmlComments(xmlFile, true);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure Hangfire (commented out for now - can be enabled in production)
// builder.Services.AddHangfire(config =>
// {
//     config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
// });
// builder.Services.AddHangfireServer();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key is not configured");
}



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };


    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("UserOrAdmin", policy => 
        policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList API V1");
    c.DocumentTitle = "TodoList API - Auto-Authenticated";
});

app.UseCors("AllowFrontend");



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// SignalR hub can be enabled when properly configured
// app.MapHub<TodoListApp.Infrastructure.Hubs.TodoItemHub>("/todoItemHub");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    await DataSeeder.SeedAsync(context, userManager, roleManager);
}

// Background jobs can be configured here when Hangfire is properly set up
// RecurringJob.AddOrUpdate<IBackgroundJobService>("cleanup-expired-tokens", service => service.CleanupExpiredRefreshTokensAsync(CancellationToken.None), Cron.Daily);

app.Run();

// Make the Program class public for integration tests
public partial class Program { }
