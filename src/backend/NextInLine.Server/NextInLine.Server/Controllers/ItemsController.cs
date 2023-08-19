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
    public ItemsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Add a new item
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody, Required] AddItemCommand command)
    { 
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }
    
    /// <summary>
    /// Add a new tag
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("tag")]
    public async Task<IActionResult> AddTag([FromBody, Required] AddTagCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.Success)
        {
            return Ok(new { Id = result.TagId, result.Message });
        }

        return BadRequest(new { Id = result.TagId, result.Message });
    }
    
    /// <summary>
    /// Check an item
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    [HttpPost("check/{itemId}")]
    public async Task<IActionResult> CheckItem([Required]int itemId)
    {
        var result = await _mediator.Send(new CheckItemCommand(itemId));

        if (result.Success)
        {
            return Ok();
        }

        return BadRequest("Failed to check the item.");
    }

    /// <summary>
    /// Modify an item
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{itemId}")]
    public async Task ModifyItem([Required]int itemId, [FromBody] ModifyItemModel model)
    {
        await _mediator.Send(new ModifyItemCommand(itemId, model.NewName, model.NewTagIds));
    }
    
    [HttpGet("{itemId}")]
    public async Task<ActionResult<Item>> GetItem(int itemId)
    {
        Item item = await _mediator.Send(new GetItemQuery(itemId));
        return Ok(item);
    }
}