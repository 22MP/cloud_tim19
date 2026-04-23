using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Users.Commands.ResetPassword
{
    internal class ResetPasswordHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    ILogger<ResetPasswordHandler> logger) : IRequestHandler<ResetPasswordCommand, Result>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var now = dateTimeProvider.UtcNow;

            var users = await userRepository.GetAllAsync(ct);
            var user = users.FirstOrDefault(u => u.PasswordResetToken == request.Token);

            if (user is null)
                return Result.Failure("Invalid or expired reset token.", ErrorType.NotFound);

            var passwordHash = passwordHasher.Hash(request.Password);
            var result = user.ResetPassword(passwordHash, request.Token, now);

            if (result.IsFailure)
                return result;

            await userRepository.SaveAsync(user, ct);

            logger.LogInformation("[USER] Password reset for {Email}.", user.Email);

            return Result.Success();
        }
    }
}
