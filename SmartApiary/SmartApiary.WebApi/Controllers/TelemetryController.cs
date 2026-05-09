using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Users.Commands;
using SmartApiary.Application.Features.Users.Commands.ManageUser;
using SmartApiary.Application.Features.Users.Queries;
using SmartApiary.WebApi.Extensions;

namespace SmartApiary.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TelemetryController : ControllerBase // TODO: IMPLEMENT FAKE FAKE FAKE nema autorizacije jos
{
    private readonly IMediator _mediator;

    public TelemetryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/telemetry/{hiveId}?from=2024-01-01&to=2024-01-31
    [HttpGet("{hiveId}")]
    //[Authorize(Roles = "Beekeeper")]
    public IActionResult GetByHive(string hiveId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var fake = Enumerable.Range(0, 10).Select(i => new
        {
            id = Guid.NewGuid().ToString(),
            hiveId = hiveId,
            weightKg = 30.0 + i * 0.5,
            humidity = 60.0 + i,
            temperature = 34.0 + i * 0.1,
            batteryPct = 80 - i,
            recordedAt = DateTime.UtcNow.AddHours(-i)
        });
        return Ok(fake);
    }

    // GET /api/telemetry/{hiveId}/latest
    [HttpGet("{hiveId}/latest")]
    //[Authorize(Roles = "Beekeeper")]
    public IActionResult GetLatest(string hiveId)
    {
        return Ok(new
        {
            hiveId = hiveId,
            weightKg = 35.5,
            humidity = 65.0,
            temperature = 34.2,
            batteryPct = 78,
            recordedAt = DateTime.UtcNow
        });
    }

}