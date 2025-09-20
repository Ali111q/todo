using MediatR;
using TodoListApp.Application.Features.Auth.Dtos;

namespace TodoListApp.Application.Features.Auth.Commands;

public sealed record RegisterCommand : IRequest<AuthResultDto>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

