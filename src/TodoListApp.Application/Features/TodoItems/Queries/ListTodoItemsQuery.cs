using MediatR;
using TodoListApp.Application.Common.Dtos;
using TodoListApp.Application.Features.TodoItems.Dtos;

namespace TodoListApp.Application.Features.TodoItems.Queries;

public sealed record ListTodoItemsQuery(
    bool? Completed, 
    DateTime? DueOnOrBefore, 
    int Page = 1, 
    int PageSize = 20, 
    string SortBy = "CreatedAt", 
    bool SortDescending = true)
    : IRequest<PagedResultDto<TodoItemDto>>;
