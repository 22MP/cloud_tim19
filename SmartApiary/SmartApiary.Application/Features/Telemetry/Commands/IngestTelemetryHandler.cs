using MediatR;
using SmartApiary.Application.Features.Telemetry.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Telemetry.Commands
{
    internal class IngestTelemetryHandler : IRequestHandler<IngestTelemetryCommand, Result>
    {
        private const double DefaultWeightDropThresholdKg = 10.0;
        private const int LowBatteryThresholdPercent = 15;

        private readonly IDeviceRepository _deviceRepository;
        private readonly ITelemetryQueueService _queueService;
        private readonly ITelemetrySampleRepository _telemetryRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly ITelemetryRealtimeNotifier _realtimeNotifier;

        public IngestTelemetryHandler(
            IDeviceRepository deviceRepository,
            ITelemetryQueueService queueService,
            ITelemetrySampleRepository telemetryRepository,
            IHiveRepository hiveRepository,
            ITelemetryRealtimeNotifier realtimeNotifier)
        {
            _deviceRepository = deviceRepository;
            _queueService = queueService;
            _telemetryRepository = telemetryRepository;
            _hiveRepository = hiveRepository;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<Result> Handle(IngestTelemetryCommand request, CancellationToken ct)
        {
            var device = await _deviceRepository.GetByAccessTokenAsync(request.AccessToken, ct);
            if (device is null) return Result.Failure("Invalid device token.", ErrorType.Unauthorized);

            var previousSample = await _telemetryRepository.GetLatestByHiveIdAsync(device.HiveId.Value, ct);

            var sample = TelemetrySample.Create(
                device.Id.Value, device.HiveId.Value,
                request.WeightKg, request.Humidity,
                request.Temperature, request.BatteryPercentage,
                request.RecordedAt
            );

            if (sample.IsFailure) return Result.Failure(sample.Error!.Message, ErrorType.Validation);

            await _queueService.EnqueueAsync(sample.Value, ct);
            await _realtimeNotifier.NotifyTelemetryReceivedAsync(sample.Value, ct);
            await NotifyAlertsAsync(sample.Value, previousSample, ct);

            return Result.Success();
        }

        private async Task NotifyAlertsAsync(TelemetrySample current, TelemetrySample? previous, CancellationToken ct)
        {
            if (previous is null)
                return;

            var hive = await _hiveRepository.GetByIdAsync(current.HiveId, ct);
            if (hive is null)
                return;

            var weightDrop = previous.WeightKg - current.WeightKg;
            if (weightDrop >= DefaultWeightDropThresholdKg)
            {
                await _realtimeNotifier.NotifyTelemetryAlertAsync(
                    hive.ApiaryId.Value,
                    new TelemetryAlertDto(
                        "WeightDrop",
                        "Critical",
                        $"Sudden weight drop detected: {weightDrop:0.##} kg. Possible theft or overturned hive.",
                        DateTime.UtcNow,
                        current.HiveId.Value,
                        current.DeviceId.Value),
                    ct);
            }

            if (previous.BatteryPercentage >= LowBatteryThresholdPercent && current.BatteryPercentage < LowBatteryThresholdPercent)
            {
                await _realtimeNotifier.NotifyTelemetryAlertAsync(
                    hive.ApiaryId.Value,
                    new TelemetryAlertDto(
                        "LowBattery",
                        "Warning",
                        $"Hive scale battery is low: {current.BatteryPercentage}%.",
                        DateTime.UtcNow,
                        current.HiveId.Value,
                        current.DeviceId.Value),
                    ct);
            }
        }
    }
}
