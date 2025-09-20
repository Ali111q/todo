using FluentValidation;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Domain.TodoItems.ValueObjects;

namespace TodoListApp.Application.Validation.TodoItems;

public sealed class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description != null);
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Priority must be a valid value (Low, Medium, High, Critical)");
    }
}
