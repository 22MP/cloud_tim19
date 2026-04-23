using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;

namespace SmartApiary.Application.Features.Users.Commands.ActivateAccount;

internal class ActivateAccountHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    ILogger<ActivateAccountHandler> logger) : IRequestHandler<ActivateAccountCommand, Result>
{
    public async Task<Result> Handle(ActivateAccountCommand request, CancellationToken ct)
    {
        var now = dateTimeProvider.UtcNow;

        var users = await userRepository.GetAllAsync(ct);
        var user = users.FirstOrDefault(u => u.ActivationToken == request.Token);

        if (user is null)
            return Result.Failure("Invalid or expired activation token.", ErrorType.NotFound);

        var passwordHash = passwordHasher.Hash(request.Password);

        var result = user.ActivateAccount(passwordHash, request.Token, now);

        if (result.IsFailure)
            return result;

        await userRepository.SaveAsync(user, ct);

        logger.LogInformation("[USER] Account activated for {Email}.", user.Email);

        return Result.Success();
    }
}