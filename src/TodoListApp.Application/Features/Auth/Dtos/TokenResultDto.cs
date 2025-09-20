namespace TodoListApp.Application.Features.Auth.Dtos;

public sealed record TokenResultDto(string AccessToken, DateTime ExpiresAtUtc);
