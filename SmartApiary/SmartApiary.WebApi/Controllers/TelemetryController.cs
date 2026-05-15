using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApiary.Application.Features.Telemetry.DTOs;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;

namespace SmartApiary.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Beekeeper")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetrySampleRepository _telemetryRepository;

    public TelemetryController(ITelemetrySampleRepository telemetryRepository)
    {
        _telemetryRepository = telemetryRepository;
    }

    [HttpGet("{hiveId}")]
    public async Task<IActionResult> GetByHive(string hiveId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        DateTime utcTo = (to ?? DateTime.UtcNow).ToUniversalTime();
        DateTime utcFrom = (from ?? utcTo.AddDays(-7)).ToUniversalTime();

        if (utcFrom > utcTo)
            return BadRequest("'from' must be before 'to'.");

        var samples = await _telemetryRepository.GetByHiveAsync(hiveId, utcFrom, utcTo, ct);

        return Ok(samples
            .OrderBy(x => x.RecordedAt)
            .Select(ToDto));
    }

    [HttpGet("{hiveId}/latest")]
    public async Task<IActionResult> GetLatest(string hiveId, CancellationToken ct)
    {
        var latest = await _telemetryRepository.GetLatestByHiveIdAsync(hiveId, ct);
        return latest is null ? NotFound() : Ok(ToDto(latest));
    }

    [HttpGet("{hiveId}/nectar-delta")]
    public async Task<IActionResult> GetNectarDelta(string hiveId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        DateTime utcTo = (to ?? DateTime.UtcNow).ToUniversalTime();
        DateTime utcFrom = (from ?? utcTo.AddDays(-14)).ToUniversalTime();

        if (utcFrom > utcTo)
            return BadRequest("'from' must be before 'to'.");

        var samples = await _telemetryRepository.GetByHiveAsync(hiveId, utcFrom.Date, utcTo.Date.AddDays(1).AddTicks(-1), ct);

        var deltas = samples
            .GroupBy(x => x.RecordedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var daySamples = g.OrderBy(x => x.RecordedAt).ToList();
                var sampleAt8 = daySamples.MinBy(x => Math.Abs((x.RecordedAt.TimeOfDay - TimeSpan.FromHours(8)).TotalMinutes));
                var sampleAt20 = daySamples.MinBy(x => Math.Abs((x.RecordedAt.TimeOfDay - TimeSpan.FromHours(20)).TotalMinutes));
                var delta = sampleAt8 is null || sampleAt20 is null ? 0 : Math.Round(sampleAt20.WeightKg - sampleAt8.WeightKg, 2);

                return new NectarDeltaDto(g.Key, sampleAt8?.WeightKg, sampleAt20?.WeightKg, delta);
            });

        return Ok(deltas);
    }

    private static TelemetrySampleDto ToDto(TelemetrySample sample) => new(
        sample.Id.Value,
        sample.DeviceId.Value,
        sample.HiveId.Value,
        sample.WeightKg,
        sample.Humidity,
        sample.Temperature,
        sample.BatteryPercentage,
        sample.RecordedAt);
}
