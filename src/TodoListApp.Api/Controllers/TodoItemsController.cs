using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Application.Features.TodoItems.Commands;
using TodoListApp.Application.Features.TodoItems.Queries;

namespace TodoListApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/todoitems")]
public sealed class TodoItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TodoItemsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Create a new todo item
    /// </summary>
    /// <param name="cmd">Todo item creation data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created todo item</returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    /// <summary>
    /// Get paginated list of todo items
    /// </summary>
    /// <param name="completed">Filter by completion status</param>
    /// <param name="dueOnOrBefore">Filter by due date</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20)</param>
    /// <param name="sortBy">Sort field: CreatedAt, Name, DueDate, Priority, Completed (default: CreatedAt)</param>
    /// <param name="sortDescending">Sort direction (default: true)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of todo items</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> List([FromQuery] bool? completed, [FromQuery] DateTime? dueOnOrBefore,
                                          [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
                                          [FromQuery] string sortBy = "CreatedAt", [FromQuery] bool sortDescending = true,
                                          CancellationToken ct = default)
        => Ok(await _mediator.Send(new ListTodoItemsQuery(completed, dueOnOrBefore, page, pageSize, sortBy, sortDescending), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoItemCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateTodoItemCommandWithId(id, cmd.Name, cmd.Description, cmd.DueDate, cmd.Priority), ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTodoItemCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new ToggleCompleteTodoItemCommand(id), ct));
}
