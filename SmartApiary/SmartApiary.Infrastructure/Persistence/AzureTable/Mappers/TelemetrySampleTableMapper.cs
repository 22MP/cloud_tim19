using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Mappers
{
    internal sealed class TelemetrySampleTableMapper : ITableMapper<TelemetrySample, TelemetrySampleEntity>
    {
        public TelemetrySample? ToDomain(TelemetrySampleEntity entity)
        {
            return TelemetrySample.Load(
                entity.Id,
                entity.DeviceId,
                entity.HiveId,
                entity.WeightKg,
                entity.Humidity,
                entity.Temperature,
                entity.BatteryPercentage,
                entity.RecordedAt
            );
        }

        public TelemetrySampleEntity ToEntity(TelemetrySample domain)
        {
            return new TelemetrySampleEntity
            {
                PartitionKey = domain.HiveId.Value,
                RowKey = (DateTime.MaxValue - domain.RecordedAt).Ticks.ToString("D20"),
                Id = domain.Id.Value,
                DeviceId = domain.DeviceId.Value,
                HiveId = domain.HiveId.Value,
                WeightKg = domain.WeightKg,
                Humidity = domain.Humidity,
                Temperature = domain.Temperature,
                BatteryPercentage = domain.BatteryPercentage,
                RecordedAt = domain.RecordedAt
            };
        }
    }
}
