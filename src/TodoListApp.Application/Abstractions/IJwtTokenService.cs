using TodoListApp.Application.Features.Auth.Dtos;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Abstractions;

public interface IJwtTokenService
{
    TokenResultDto CreateToken(User user, DateTime nowUtc);
}

