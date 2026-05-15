namespace SmartApiary.Application.Interfaces;

public interface IEmailService
{
    Task SendActivationEmailAsync(
        string email,
        string firstName,
        string activationLink,
        CancellationToken ct = default);

    Task SendPasswordResetEmailAsync(
        string email,
        string firstName,
        string resetLink,
        CancellationToken ct = default);

    Task SendWelcomeEmailAsync(
    string email,
    string firstName,
    CancellationToken ct = default);

    Task SendAlertEmailAsync(
        string email,
        string subject,
        string htmlMessage,
        CancellationToken ct = default);
}