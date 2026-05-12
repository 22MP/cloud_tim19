using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Messages
{
    public class TelemetryQueueMessage
    {
        public string DeviceId { get; init; } = string.Empty;
        public string HiveId { get; init; } = string.Empty;
        public double WeightKg { get; init; }
        public double Humidity { get; init; }
        public double Temperature { get; init; }
        public int BatteryPercentage { get; init; }
        public DateTime RecordedAt { get; init; }
    }
}
