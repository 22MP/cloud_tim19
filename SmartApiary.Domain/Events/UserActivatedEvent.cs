using SmartApiary.Domain.Common;

namespace SmartApiary.Domain.Events;

public record UserActivatedEvent(string UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}