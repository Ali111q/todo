using MediatR;
using TodoListApp.Application.Features.TodoItems.Dtos;

namespace TodoListApp.Application.Features.TodoItems.Queries;

public sealed record ListTagsQuery : IRequest<List<TagDto>>;
