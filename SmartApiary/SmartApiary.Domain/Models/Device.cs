using SmartApiary.Domain.Common;
using SmartApiary.Domain.Constants;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Devices;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class Device : AggregateRoot
    {
        public EntityId Id { get; private set; }
        public EntityId HiveId { get; private set; }
        public string SerialNumber { get; private set; } = string.Empty;
        public string? DeviceUuid { get; private set; }
        public string? AccessToken { get; private set; }
        public DeviceStatus Status { get; private set; }
        
        public DateTime RegisteredAt { get; private set; }
        public DateTime? PairedAt { get; private set; }

        private Device(EntityId id, EntityId hiveId, string serialNumber, DateTime registeredAt)
        {
            Id = id; HiveId = hiveId;
            SerialNumber = serialNumber;
            Status = DeviceStatus.Unpaired;
            RegisteredAt = registeredAt;
        }

        public static Result<Device> Create(string hiveId, string serialNumber, DateTime registeredAt)
        {
            if (!IsValidSerialNumber(serialNumber)) return Result<Device>.Failure("Invalid serial number format (SA-YYYY-XXXXX).", ErrorType.Validation);
            Result<EntityId> hiveIdResult = EntityId.Create(hiveId);
            if (hiveIdResult.IsFailure) return Result<Device>.Failure(hiveIdResult.Error!.Message, ErrorType.Validation);
            return Result<Device>.Success(new Device(EntityId.New(), hiveIdResult.Value, serialNumber.ToUpperInvariant(), registeredAt));
        }
        
        public static Result<Device> Load(string id, string hiveId, string serialNumber, string? deviceUuid, string? accessToken, DeviceStatus status, DateTime registeredAt, DateTime? pairedAt)
        {
            Result<EntityId> idResult = EntityId.Create(id);
            Result<EntityId> hiveIdResult = EntityId.Create(hiveId);
            if (idResult.IsFailure) return Result<Device>.Failure(idResult.Error!.Message, ErrorType.Validation);
            
            if (hiveIdResult.IsFailure) return Result<Device>.Failure(hiveIdResult.Error!.Message, ErrorType.Validation);
            
            return Result<Device>.Success( new Device(idResult.Value, hiveIdResult.Value, serialNumber, registeredAt)
            {
                DeviceUuid = deviceUuid,
                AccessToken = accessToken,
                Status = status,
                PairedAt = pairedAt
            });

        }

        public Result CompleteHandshake(string deviceUuid, string accessToken, DateTime pairedAt)
        {
            if (Status == DeviceStatus.Paired) return Result.Failure("Device is already paired.", ErrorType.Validation);
            
            DeviceUuid = deviceUuid;
            AccessToken = accessToken;
            Status = DeviceStatus.Paired;
            PairedAt = pairedAt;
            return Result.Success();
        }

        private static bool IsValidSerialNumber(string sn)
        {
            if (string.IsNullOrWhiteSpace(sn)) return false;
            var parts = sn.ToUpperInvariant().Split('-');
            return parts.Length == 3 && parts[0] == "SA" && int.TryParse(parts[1], out int yr) && yr >= DeviceParameters.SERIAL_NUMBER_MIN_YEAR && yr <= DeviceParameters.SERIAL_NUMBER_MAX_YEAR && parts[2].Length == 5 && parts[2].All(char.IsLetterOrDigit);
        }

    }
}
