using MediatR;
using TodoListApp.Application.Features.TodoItems.Dtos;

namespace TodoListApp.Application.Features.TodoItems.Commands;

public sealed record CreateTagCommand(string Name, string Color = "#007bff") : IRequest<TagDto>;
