using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.ValueObjects;


namespace SmartApiary.Application.Features.Users.Commands.ManageUser
{
    public record SuspendUserCommand(string UserId) : IRequest<Result>;
    public class SuspendUserValidator : AbstractValidator<SuspendUserCommand>
    {
        public SuspendUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }

    internal class SuspendUserHandler(
        IUserRepository userRepository,
        ILogger<SuspendUserHandler> logger) : IRequestHandler<SuspendUserCommand, Result>
    {
        public async Task<Result> Handle(SuspendUserCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.UserId);
            if (idResult.IsFailure)
                return Result.Failure(idResult.Error!.Message, ErrorType.Validation);

            var user = await userRepository.GetByIdAsync(idResult.Value, ct);
            if (user is null)
                return Result.Failure("User not found.", ErrorType.NotFound);

            var result = user.Suspend();
            if (result.IsFailure)
                return result;

            await userRepository.SaveAsync(user, ct);

            logger.LogInformation("[USER] User {Email} suspended.", user.Email);
            return Result.Success();
        }
    }
}
