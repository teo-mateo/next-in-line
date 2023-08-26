using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NextInLine.Server.Controllers.Models;
using NextInLine.Server.CQRS.Commands;
using NextInLine.Server.CQRS.Queries;
using NextInLine.Server.CQRS.Queries.Models;

namespace NextInLine.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Add a new item
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] [Required] AddItemCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }

    /// <summary>
    ///     Add a new tag
    /// </summary>
    [HttpPost("tag")]
    public async Task<IActionResult> AddTag([FromBody] [Required] AddTagCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Success
            ? Ok(new { Id = result.TagId, result.Message })
            : BadRequest(new { Id = result.TagId, result.Message });
    }

    /// <summary>
    /// Check an item
    /// </summary>
    [HttpPost("check/{itemId:int}")]
    public async Task<IActionResult> CheckItem([Required] int itemId)
    {
        var result = await _mediator.Send(new CheckItemCommand(itemId));
        return result.Success 
            ? Ok() 
            : BadRequest("Failed to check the item.");
    }

    /// <summary>
    /// Modify an item
    /// </summary>
    [HttpPut("{itemId:int}")]
    public async Task ModifyItem([Required] int itemId, [FromBody] ModifyItemModel model)
    {
        await _mediator.Send(new ModifyItemCommand(itemId, model.NewName, model.NewTagIds));
    }

    /// <summary>
    /// Gets a single item
    /// </summary>
    [HttpGet("{itemId}")]
    public async Task<ActionResult<Item>> GetItem(int itemId)
    {
        var item = await _mediator.Send(new GetItemQuery(itemId));
        return Ok(item);
    }
    
    /// <summary>
    /// Gets unchecked items by tag id
    /// </summary>
    [HttpGet("unchecked/{tagId:int}")]
    public async Task<ActionResult<IEnumerable<Item>>> GetUncheckedItemsByTag(int tagId)
    {
        var items = await _mediator.Send(new GetUncheckedItemsByTagQuery(tagId));
        return Ok(items);
    }
}