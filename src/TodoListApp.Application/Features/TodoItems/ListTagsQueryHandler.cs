using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Application.Features.TodoItems.Queries;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class ListTagsQueryHandler : IRequestHandler<ListTagsQuery, List<TagDto>>
{
    private readonly IRepository<Tag> _tagRepo;
    private readonly ICurrentUser _currentUser;

    public ListTagsQueryHandler(IRepository<Tag> tagRepo, ICurrentUser currentUser)
    {
        _tagRepo = tagRepo;
        _currentUser = currentUser;
    }

    public async Task<List<TagDto>> Handle(ListTagsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

        // Get all tags for the current user
        var tags = await _tagRepo.Query(new UserTagsSpec(userId))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return tags.Select(t => new TagDto(t.Id, t.Name, t.Color)).ToList();
    }
}

// Simple specification for user's tags
public class UserTagsSpec : TodoListApp.Domain.Abstractions.Specification<Tag>
{
    public UserTagsSpec(Guid userId)
    {
        Criteria = t => t.UserId == userId;
    }
}
