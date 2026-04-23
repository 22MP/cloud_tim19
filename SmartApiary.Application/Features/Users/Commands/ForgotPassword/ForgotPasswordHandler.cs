using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartApiary.Application.Common.Options;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Users.Commands.ForgotPassword
{
    internal class ForgotPasswordHandler(
    IUserRepository userRepository,
    IEmailService emailService,
    IDateTimeProvider dateTimeProvider,
    IOptions<FrontendOptions> frontendOptions,
    ILogger<ForgotPasswordHandler> logger) : IRequestHandler<ForgotPasswordCommand, Result>
    {
        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null)
                return Result.Success();

            var now = dateTimeProvider.UtcNow;
            user.GeneratePasswordResetToken(now);

            await userRepository.SaveAsync(user, ct);

            var resetLink =
                $"{frontendOptions.Value.BaseUrl}/reset-password?token={user.PasswordResetToken}";

            await emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.FirstName,
                resetLink,
                ct);

            logger.LogInformation("[USER] Password reset email sent to {Email}.", user.Email);

            return Result.Success();
        }
    }
}
