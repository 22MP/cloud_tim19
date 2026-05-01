using SmartApiary.Domain.Common;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Events
{
    public record ApiaryCreatedDomainEvent(EntityId ApiaryId, string OwnerId, DateTime OccurredAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
