using SmartApiary.Application.Features.Telemetry.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Models;

namespace SmartApiary.Functions.Services;

public sealed class NoOpTelemetryRealtimeNotifier : ITelemetryRealtimeNotifier
{
    public Task NotifyTelemetryReceivedAsync(
        TelemetrySample sample,
        CancellationToken ct = default)
        => Task.CompletedTask;

    public Task NotifyTelemetryAlertAsync(
        string apiaryId,
        TelemetryAlertDto alert,
        CancellationToken ct = default)
        => Task.CompletedTask;
}