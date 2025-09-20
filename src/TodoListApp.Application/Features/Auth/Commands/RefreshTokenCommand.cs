using MediatR;
using TodoListApp.Application.Features.Auth.Dtos;

namespace TodoListApp.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResultDto>;
