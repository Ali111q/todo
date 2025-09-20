using MediatR;
using TodoListApp.Application.Abstractions;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Features.TodoItems.Dtos;
using TodoListApp.Domain.Abstractions;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Application.Features.TodoItems;

public sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly IRepository<Tag> _tagRepo;
    private readonly ICurrentUser _currentUser;

    public CreateTagCommandHandler(IRepository<Tag> tagRepo, ICurrentUser currentUser)
    {
        _tagRepo = tagRepo;
        _currentUser = currentUser;
    }

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        
        var tag = Tag.Create(request.Name, request.Color, userId);
        
        await _tagRepo.AddAsync(tag, cancellationToken);
        await _tagRepo.SaveChangesAsync(cancellationToken);

        return new TagDto(tag.Id, tag.Name, tag.Color);
    }
}
