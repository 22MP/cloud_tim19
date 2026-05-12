using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Services
{
    internal class TelemetryQueueService : AzureQueueService<TelemetryQueueMessage>, ITelemetryQueueService
    {

        public TelemetryQueueService(
            QueueServiceClient queueServiceClient, IJsonSerializer serializer, ILogger<TelemetryQueueService> logger
        ) : base(queueServiceClient.GetQueueClient("telemetry-queue"), serializer, logger)
        {

        }

        public async Task EnqueueAsync(TelemetrySample sample, CancellationToken ct = default)
        {
            await SendMessageAsync(new TelemetryQueueMessage
            {
                DeviceId = sample.DeviceId.Value,
                HiveId = sample.HiveId.Value,
                WeightKg = sample.WeightKg,
                Humidity = sample.Humidity,
                Temperature = sample.Temperature,
                BatteryPercentage = sample.BatteryPercentage,
                RecordedAt = sample.RecordedAt
            }, ct);
        }
    }
}
