using SmartApiary.Domain.Common;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Events
{
    public record DevicePairedEvent(EntityId DeviceId, EntityId HiveId, string AccessToken, string DeviceUuid, DateTime PairedAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
