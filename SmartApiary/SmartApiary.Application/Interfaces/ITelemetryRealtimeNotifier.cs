using SmartApiary.Application.Features.Telemetry.DTOs;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Interfaces;

public interface ITelemetryRealtimeNotifier
{
    Task NotifyTelemetryReceivedAsync(TelemetrySample sample, CancellationToken ct = default);
    Task NotifyTelemetryAlertAsync(string apiaryId, TelemetryAlertDto alert, CancellationToken ct = default);
}
