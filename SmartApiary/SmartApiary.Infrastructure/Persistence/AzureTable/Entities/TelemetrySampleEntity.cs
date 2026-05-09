using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Entities
{
    internal class TelemetrySampleEntity : BaseTableEntity
    {
        public string Id { get; set; } = default!;
        public string DeviceId { get; set; } = default!;
        public string HiveId { get; set; } = default!;
        public double WeightKg { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public int BatteryPercentage { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
