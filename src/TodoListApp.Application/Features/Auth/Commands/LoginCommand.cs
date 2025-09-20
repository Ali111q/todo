using MediatR;
using TodoListApp.Application.Features.Auth.Dtos;

namespace TodoListApp.Application.Features.Auth.Commands;

public sealed record LoginCommand(string UsernameOrEmail, string Password) : IRequest<AuthResultDto>;

