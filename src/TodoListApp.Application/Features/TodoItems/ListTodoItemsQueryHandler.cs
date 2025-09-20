using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Common.Dtos;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Application.Features.TodoItems.Queries;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.Specifications;
using TodoListApp.Domain.TodoItems;
using TodoListApp.Domain.Users;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class ListTodoItemsQueryHandler : IRequestHandler<ListTodoItemsQuery, PagedResultDto<TodoItemDto>>
{
    private readonly IRepository<TodoItem> _repo;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _current;

    public ListTodoItemsQueryHandler(IRepository<TodoItem> repo, UserManager<User> userManager, ICurrentUser current)
    { _repo = repo; _userManager = userManager; _current = current; }

    public async Task<PagedResultDto<TodoItemDto>> Handle(ListTodoItemsQuery request, CancellationToken ct)
    {
        var ownerId = _current.UserId ?? throw new UnauthorizedAccessException();

        var skip = (request.Page - 1) * request.PageSize;

        var countSpec = new TodoItemByFiltersSpec(ownerId, request.Completed, request.DueOnOrBefore, request.DueDateFrom, request.DueDateTo, request.Priority, request.SearchText, request.TagIds, null, null, request.SortBy, request.SortDescending);
        var totalCount = await _repo.Query(countSpec).CountAsync(ct);

        var dataSpec = new TodoItemByFiltersSpec(ownerId, request.Completed, request.DueOnOrBefore, request.DueDateFrom, request.DueDateTo, request.Priority, request.SearchText, request.TagIds, skip, request.PageSize, request.SortBy, request.SortDescending);
        var items = await _repo.Query(dataSpec)
            .Include(t => t.TodoItemTags)
            .ThenInclude(tt => tt.Tag)
            .ToListAsync(ct);

        var userIds = items.Select(t => t.UserId).Distinct().ToList();
        var userDict = new Dictionary<Guid, string>();

        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                userDict[userId] = user.UserName ?? "Unknown User";
            }
        }

        var todoItemDtos = items.Select(t => new TodoItemDto(
            t.Id,
            t.UserId,
            userDict.GetValueOrDefault(t.UserId, "Unknown User"),
            t.Name.Value,
            t.Description.Value,
            t.DueDate.Value,
            t.Priority.Value,
            t.IsCompleted,
            t.CreatedAt,
            t.CompletedAtUtc,
            t.TodoItemTags.Select(tt => new TagDto(tt.Tag.Id, tt.Tag.Name, tt.Tag.Color)).ToList())).ToList();

        return PagedResultDto<TodoItemDto>.Create(todoItemDtos, totalCount, request.Page, request.PageSize);
    }
}
