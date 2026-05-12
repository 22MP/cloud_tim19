using Azure.Storage.Queues.Models;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Messages;
using System.Text.Json;

namespace SmartApiary.Functions.Ingestion;

public class TelemetryQueueFunction
{
    private readonly ITelemetrySampleRepository _telemetrySampleRepository;
    private readonly IJsonSerializer _serializer;
    private readonly ILogger<TelemetryQueueFunction> _logger;

    public TelemetryQueueFunction(ITelemetrySampleRepository telemetrySampleRepository, IJsonSerializer serializer, ILogger<TelemetryQueueFunction> logger)
    {
        _telemetrySampleRepository = telemetrySampleRepository;
        _serializer = serializer;
        _logger = logger;
    }

    [Function("TelemetryQueueFunction")]
    public async Task Run([QueueTrigger("telemetry-queue")] string message)
    {
        TelemetryQueueMessage? msg = _serializer.Deserialize<TelemetryQueueMessage>(message);

        if (msg is null)
        {
            _logger.LogError("Failed to deserialize telemetry message.");
            return;
        }

        // TEMP
        _logger.LogInformation("[DEBUG] DeviceId: '{DeviceId}'", msg.DeviceId);
        _logger.LogInformation("[DEBUG] HiveId: '{HiveId}'", msg.HiveId);


        var sample = TelemetrySample.Create(
            msg.DeviceId,
            msg.HiveId,
            msg.WeightKg,
            msg.Humidity,
            msg.Temperature,
            msg.BatteryPercentage,
            msg.RecordedAt
        );

        if (sample.IsFailure)
        {
            _logger.LogError("Failed to create TelemetrySample: {Error}", sample.Error?.Message);
            return;
        }

        await _telemetrySampleRepository.AddAsync(sample.Value);
        _logger.LogInformation("Telemetry saved for hive {HiveId}", msg.HiveId);
    }
}