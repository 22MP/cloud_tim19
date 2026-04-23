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
    public record DeleteUserCommand(string UserId) : IRequest<Result>;

    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }

    internal class DeleteUserHandler(
        IUserRepository userRepository,
        ILogger<DeleteUserHandler> logger) : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.UserId);
            if (idResult.IsFailure)
                return Result.Failure(idResult.Error!.Message, ErrorType.Validation);

            var user = await userRepository.GetByIdAsync(idResult.Value, ct);
            if (user is null)
                return Result.Failure("User not found.", ErrorType.NotFound);

            await userRepository.DeleteAsync(idResult.Value, ct);

            logger.LogInformation("[USER] User {Email} deleted.", user.Email);
            return Result.Success();
        }
    }
}
