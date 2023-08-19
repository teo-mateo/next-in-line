using MediatR;
using Microsoft.AspNetCore.Mvc;
using NextInLine.Server.CQRS.Queries;
using NextInLine.Server.CQRS.Queries.Models;

namespace NextInLine.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all tags
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        IEnumerable<Tag> tags = await _mediator.Send(new GetTagsQuery());
        return Ok(tags);
    }
}