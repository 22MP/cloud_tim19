using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class TelemetrySample
    {
        public EntityId Id { get; private set; }
        public EntityId DeviceId { get; private set; }
        public EntityId HiveId { get; private set; }
        public double WeightKg { get; private set; }
        public double Humidity { get; private set; }
        public double Temperature { get; private set; }
        public int BatteryPercentage { get; private set; }
       
        public DateTime RecordedAt { get; private set; }
        
        private TelemetrySample(EntityId id, EntityId deviceId, EntityId hiveId, double weightKg, double humidity, double temperature, int batteryPct, DateTime recordedAt)
        {
            Id = id; DeviceId = deviceId; HiveId = hiveId;
            WeightKg = weightKg; Humidity = humidity;
            Temperature = temperature; BatteryPercentage = batteryPct;
            RecordedAt = recordedAt;
        }

        public static Result<TelemetrySample> Create(string deviceId, string hiveId, double weightKg, double humidity, double temperature, int batteryPct, DateTime recordedAt)
        {
            Result<EntityId> deviceResult = EntityId.Create(deviceId);
            Result<EntityId> hiveResult = EntityId.Create(hiveId);

            if (deviceResult.IsFailure) return Result<TelemetrySample>.Failure(deviceResult.Error!.Message, ErrorType.Validation);

            if (hiveResult.IsFailure) return Result<TelemetrySample>.Failure(hiveResult.Error!.Message, ErrorType.Validation);

            if (batteryPct < 0 || batteryPct > 100) return Result<TelemetrySample>.Failure("Battery percentage must be in 0-100 range.", ErrorType.Validation);

            return Result<TelemetrySample>.Success(new TelemetrySample(EntityId.New(), deviceResult.Value, hiveResult.Value, weightKg, humidity, temperature, batteryPct, recordedAt));
        }

        public static TelemetrySample Load(string id, string deviceId, string hiveId, double weightKg, double humidity, double temperature, int batteryPct, DateTime recordedAt)
        {
            return new TelemetrySample(
                EntityId.Create(id).Value, 
                EntityId.Create(deviceId).Value, 
                EntityId.Create(hiveId).Value, 
                weightKg, humidity, temperature, batteryPct, recordedAt
            );
        }
    }

}
