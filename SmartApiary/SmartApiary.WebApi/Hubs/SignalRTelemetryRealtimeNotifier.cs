using Microsoft.AspNetCore.SignalR;
using SmartApiary.Application.Features.Telemetry.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;

namespace SmartApiary.WebApi.Hubs;

public sealed class SignalRTelemetryRealtimeNotifier : ITelemetryRealtimeNotifier
{
    private readonly IHubContext<TelemetryHub> _hubContext;
    private readonly IHiveRepository _hiveRepository;

    public SignalRTelemetryRealtimeNotifier(IHubContext<TelemetryHub> hubContext, IHiveRepository hiveRepository)
    {
        _hubContext = hubContext;
        _hiveRepository = hiveRepository;
    }

    public async Task NotifyTelemetryReceivedAsync(TelemetrySample sample, CancellationToken ct = default)
    {
        var hive = await _hiveRepository.GetByIdAsync(sample.HiveId, ct);
        if (hive is null)
            return;

        await _hubContext.Clients
            .Group(TelemetryHub.GetApiaryGroupName(hive.ApiaryId.Value))
            .SendAsync("TelemetryReceived", ToDto(sample), ct);
    }

    public Task NotifyTelemetryAlertAsync(string apiaryId, TelemetryAlertDto alert, CancellationToken ct = default)
    {
        return _hubContext.Clients
            .Group(TelemetryHub.GetApiaryGroupName(apiaryId))
            .SendAsync("TelemetryAlert", alert, ct);
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
