using SmartApiary.Domain.Common;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Events
{
    public record PlotCreatedDomainEvent(EntityId PlotId, string OwnerId, DateTime OccurredAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SprayingScheduledDomainEvent(
        EntityId ScheduleId,
        EntityId PlotId,
        string OwnerId,
        DateTime ScheduledAt,
        int DurationHours,
        double PlotLatitude,
        double PlotLongitude,
        DateTime OccurredAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SprayingCancelledDomainEvent(
        EntityId ScheduleId,
        EntityId PlotId,
        string OwnerId,
        DateTime OccurredAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SprayingRescheduledDomainEvent(
        EntityId ScheduleId,
        EntityId PlotId,
        string OwnerId,
        DateTime NewScheduledAt,
        double PlotLatitude,
        double PlotLongitude,
        DateTime OccurredAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
