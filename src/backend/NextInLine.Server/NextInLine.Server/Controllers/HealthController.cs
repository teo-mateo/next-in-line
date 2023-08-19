using MediatR;
using Microsoft.AspNetCore.Mvc;
using NextInLine.Server.CQRS.Queries;

namespace NextInLine.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly IMediator _mediator;

    public HealthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> CheckHealth()
    {
        var result = await _mediator.Send(new HealthQuery());

        return result.IsHealthy
            ? Ok(new { Status = "Healthy", result.Message })
            : StatusCode(500,
                new { Status = "Unhealthy", result.Message });
    }
}