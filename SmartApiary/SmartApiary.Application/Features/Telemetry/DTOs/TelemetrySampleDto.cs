namespace SmartApiary.Application.Features.Telemetry.DTOs;

public sealed record TelemetrySampleDto(
    string Id,
    string DeviceId,
    string HiveId,
    double WeightKg,
    double Humidity,
    double Temperature,
    int BatteryPercentage,
    DateTime RecordedAt);

public sealed record NectarDeltaDto(
    DateTime Date,
    double? WeightAt8h,
    double? WeightAt20h,
    double DeltaKg);

public sealed record TelemetryAlertDto(
    string Type,
    string Severity,
    string Message,
    DateTime Timestamp,
    string HiveId,
    string? DeviceId = null);
