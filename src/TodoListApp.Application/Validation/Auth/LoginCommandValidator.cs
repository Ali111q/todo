using FluentValidation;
using TodoListApp.Application.Features.Auth.Commands;

namespace TodoListApp.Application.Validation.Auth;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

