using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.Hives.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Apiaries;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Hives.Commands.UpdateHive
{
    // COMMAND
    public record UpdateHiveCommand : IRequest<Result<HiveDto>>
    {
        public string HiveId { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public HiveType Type { get; init; }
        public string? BoxColor { get; init; }
        public int? QueenAge { get; init; }
        public string? Note { get; init; }
    }

    // VALIDATOR
    public class UpdateHiveValidator : AbstractValidator<UpdateHiveCommand>
    {
        public UpdateHiveValidator()
        {
            RuleFor(x => x.HiveId)
                .NotEmpty().WithMessage("Hive ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid hive ID format.");

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Hive label is required.")
                .MaximumLength(50).WithMessage("Hive label must not exceed 50 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid hive type.")
                .NotEqual(HiveType.Unknown).WithMessage("Hive type must be specified.");

            RuleFor(x => x.QueenAge)
                .GreaterThanOrEqualTo(0).WithMessage("Queen age cannot be negative.")
                .When(x => x.QueenAge.HasValue);
        }
    }

    // HANDLER
    internal class UpdateHiveHandler(
        IApiaryRepository apiaryRepository,
        IHiveRepository hiveRepository,
        ICurrentUserService currentUserService,
        IMapper<Hive, HiveDto> mapper,
        ILogger<UpdateHiveHandler> logger) : IRequestHandler<UpdateHiveCommand, Result<HiveDto>>
    {
        public async Task<Result<HiveDto>> Handle(UpdateHiveCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.HiveId);
            if (idResult.IsFailure)
                return Result<HiveDto>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var hive = await hiveRepository.GetByIdAsync(idResult.Value, ct);
            if (hive is null)
                return Result<HiveDto>.Failure("Hive not found.", ErrorType.NotFound);

            // Verify ownership through apiary
            var apiary = await apiaryRepository.GetByIdAsync(hive.ApiaryId, ct);
            if (apiary is null || !apiary.IsOwnedBy(currentUserService.UserId))
                return Result<HiveDto>.Failure("Access denied.", ErrorType.Unauthorized);

            var updateResult = hive.Update(request.Label, request.Type, request.BoxColor, request.QueenAge, request.Note);
            if (updateResult.IsFailure)
                return Result<HiveDto>.Failure(updateResult.Error!.Message, ErrorType.Validation);

            try
            {
                await hiveRepository.UpdateAsync(hive, ct);

                logger.LogInformation("[HIVE] Hive {Id} updated.", request.HiveId);

                return Result<HiveDto>.Success(mapper.Map(hive));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[HIVE] Error updating hive {Id}.", request.HiveId);
                return Result<HiveDto>.Failure("Failed to update hive.", ErrorType.Failure);
            }
        }
    }
}
