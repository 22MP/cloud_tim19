using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Common;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Events;

namespace SmartApiary.Application.Features.Users.EventHandlers;

public class UserActivatedEventHandler(
    IEmailService emailService,
    ILogger<UserActivatedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<UserActivatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<UserActivatedEvent> notification,
        CancellationToken ct)
    {
        var evt = notification.Event;

        await emailService.SendWelcomeEmailAsync(
            evt.Email,
            evt.FirstName,
            ct);

        logger.LogInformation(
            "[USER] Welcome email sent to {Email}.",
            evt.Email);
    }
}