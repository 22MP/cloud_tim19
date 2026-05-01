using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Apiaries.Commands.DeleteApiary
{
    // COMMAND
    public record DeleteApiaryCommand(string ApiaryId) : IRequest<Result>;

    // VALIDATOR
    public class DeleteApiaryValidator : AbstractValidator<DeleteApiaryCommand>
    {
        public DeleteApiaryValidator()
        {
            RuleFor(x => x.ApiaryId)
                .NotEmpty().WithMessage("Apiary ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid apiary ID format.");
        }
    }

    // HANDLER
    internal class DeleteApiaryHandler(
        IApiaryRepository apiaryRepository,
        IApiaryImageStorage imageStorage,
        ICurrentUserService currentUserService,
        ILogger<DeleteApiaryHandler> logger) : IRequestHandler<DeleteApiaryCommand, Result>
    {
        public async Task<Result> Handle(DeleteApiaryCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.ApiaryId);
            if (idResult.IsFailure)
                return Result.Failure(idResult.Error!.Message, ErrorType.Validation);

            var apiary = await apiaryRepository.GetByIdAsync(idResult.Value, ct);
            if (apiary is null)
                return Result.Failure("Apiary not found.", ErrorType.NotFound);

            if (!apiary.IsOwnedBy(currentUserService.UserId))
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            try
            {
                if (apiary.ImageUrl is not null)
                    await imageStorage.DeleteAllAsync(apiary.Id.Value, ct);

                await apiaryRepository.DeleteAsync(idResult.Value, ct);

                logger.LogInformation("[APIARY] Apiary {Id} deleted by user {OwnerId}.",
                    request.ApiaryId, currentUserService.UserId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[APIARY] Error deleting apiary {Id}.", request.ApiaryId);
                return Result.Failure("Failed to delete apiary.", ErrorType.Failure);
            }
        }
    }
}
