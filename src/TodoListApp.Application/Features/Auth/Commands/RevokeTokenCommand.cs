using MediatR;

namespace TodoListApp.Application.Features.Auth.Commands;

public sealed record RevokeTokenCommand(string RefreshToken) : IRequest;
