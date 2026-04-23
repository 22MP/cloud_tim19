using MediatR;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Common;
using SmartApiary.Application.Common.Options;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Events;

namespace SmartApiary.Application.Features.Users.EventHandlers;

public class UserCreatedEventHandler(
    IEmailService emailService,
    IOptions<FrontendOptions> frontendOptions)
    : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<UserCreatedEvent> notification,
        CancellationToken ct)
    {
        var evt = notification.Event;

        var activationLink =
            $"{frontendOptions.Value.BaseUrl}/activate?token={evt.ActivationToken}";

        await emailService.SendActivationEmailAsync(
            evt.Email,
            evt.FirstName,
            activationLink,
            ct);
    }
}