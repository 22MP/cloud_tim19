using MediatR;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Telemetry.Commands
{
    internal class IngestTelemetryHandler : IRequestHandler<IngestTelemetryCommand, Result>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly ITelemetryQueueService _queueService;

        public IngestTelemetryHandler(IDeviceRepository deviceRepository, ITelemetryQueueService queueService)
        {
            _deviceRepository = deviceRepository;
            _queueService = queueService;
        }

        public async Task<Result> Handle(IngestTelemetryCommand request, CancellationToken ct)
        {
            var device = await _deviceRepository.GetByAccessTokenAsync(request.AccessToken, ct);
            if (device is null) return Result.Failure("Invalid device token.", ErrorType.Unauthorized);

            var sample = TelemetrySample.Create(
                device.Id.Value, device.HiveId.Value,
                request.WeightKg, request.Humidity,
                request.Temperature, request.BatteryPercentage,
                request.RecordedAt
            );

            if (sample.IsFailure) return Result.Failure(sample.Error!.Message, ErrorType.Validation);

            await _queueService.EnqueueAsync(sample.Value, ct);

            return Result.Success();
        }
    }
}
