using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Features.TodoItems.Queries;

namespace TodoListApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/tags")]
public sealed class TagsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public TagsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Get all tags for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetTags(CancellationToken ct)
        => Ok(await _mediator.Send(new ListTagsQuery(), ct));

    /// <summary>
    /// Create a new tag
    /// </summary>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Create([FromBody] CreateTagCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    /// <summary>
    /// Add tag to todo item
    /// </summary>
    [HttpPost("todo-items/{todoItemId:guid}/tags/{tagId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AddTagToTodoItem(Guid todoItemId, Guid tagId, CancellationToken ct)
    {
        await _mediator.Send(new AddTagToTodoItemCommand(todoItemId, tagId), ct);
        return Ok(new { message = "Tag added successfully" });
    }

    /// <summary>
    /// Remove tag from todo item
    /// </summary>
    [HttpDelete("todo-items/{todoItemId:guid}/tags/{tagId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RemoveTagFromTodoItem(Guid todoItemId, Guid tagId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveTagFromTodoItemCommand(todoItemId, tagId), ct);
        return Ok(new { message = "Tag removed successfully" });
    }
}
