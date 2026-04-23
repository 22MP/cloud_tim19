using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Users.Commands.ManageUser
{
    public record ReactivateUserCommand(string UserId) : IRequest<Result>;

    public class ReactivateUserValidator : AbstractValidator<ReactivateUserCommand>
    {
        public ReactivateUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }

    internal class ReactivateUserHandler(
        IUserRepository userRepository,
        ILogger<ReactivateUserHandler> logger) : IRequestHandler<ReactivateUserCommand, Result>
    {
        public async Task<Result> Handle(ReactivateUserCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.UserId);
            if (idResult.IsFailure)
                return Result.Failure(idResult.Error!.Message, ErrorType.Validation);

            var user = await userRepository.GetByIdAsync(idResult.Value, ct);
            if (user is null)
                return Result.Failure("User not found.", ErrorType.NotFound);

            var result = user.Reactivate();
            if (result.IsFailure)
                return result;

            await userRepository.SaveAsync(user, ct);

            logger.LogInformation("[USER] User {Email} reactivated.", user.Email);
            return Result.Success();
        }
    }
}
