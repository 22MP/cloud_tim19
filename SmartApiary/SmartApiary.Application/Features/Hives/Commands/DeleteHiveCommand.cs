using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Hives.Commands.DeleteHive
{
    // COMMAND
    public record DeleteHiveCommand(string HiveId) : IRequest<Result>;

    // VALIDATOR
    public class DeleteHiveValidator : AbstractValidator<DeleteHiveCommand>
    {
        public DeleteHiveValidator()
        {
            RuleFor(x => x.HiveId)
                .NotEmpty().WithMessage("Hive ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid hive ID format.");
        }
    }

    // HANDLER
    internal class DeleteHiveHandler(
        IApiaryRepository apiaryRepository,
        IHiveRepository hiveRepository,
        ICurrentUserService currentUserService,
        ILogger<DeleteHiveHandler> logger) : IRequestHandler<DeleteHiveCommand, Result>
    {
        public async Task<Result> Handle(DeleteHiveCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.HiveId);
            if (idResult.IsFailure)
                return Result.Failure(idResult.Error!.Message, ErrorType.Validation);

            var hive = await hiveRepository.GetByIdAsync(idResult.Value, ct);
            if (hive is null)
                return Result.Failure("Hive not found.", ErrorType.NotFound);

            var apiary = await apiaryRepository.GetByIdAsync(hive.ApiaryId, ct);
            if (apiary is null || !apiary.IsOwnedBy(currentUserService.UserId))
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            try
            {
                await hiveRepository.DeleteAsync(idResult.Value, ct);

                logger.LogInformation("[HIVE] Hive {Id} deleted.", request.HiveId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[HIVE] Error deleting hive {Id}.", request.HiveId);
                return Result.Failure("Failed to delete hive.", ErrorType.Failure);
            }
        }
    }
}
