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

namespace SmartApiary.Application.Features.Hives.Commands.AddHive
{
    // COMMAND
    public record AddHiveCommand : IRequest<Result<HiveDto>>
    {
        public string ApiaryId { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public HiveType Type { get; init; }
        public string? BoxColor { get; init; }
        public int? QueenAge { get; init; }
        public string? Note { get; init; }
    }

    // VALIDATOR
    public class AddHiveValidator : AbstractValidator<AddHiveCommand>
    {
        public AddHiveValidator()
        {
            RuleFor(x => x.ApiaryId)
                .NotEmpty().WithMessage("Apiary ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid apiary ID format.");

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
    internal class AddHiveHandler(
        IApiaryRepository apiaryRepository,
        IHiveRepository hiveRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IMapper<Hive, HiveDto> mapper,
        ILogger<AddHiveHandler> logger) : IRequestHandler<AddHiveCommand, Result<HiveDto>>
    {
        public async Task<Result<HiveDto>> Handle(AddHiveCommand request, CancellationToken ct)
        {
            var apiaryIdResult = EntityId.Create(request.ApiaryId);
            if (apiaryIdResult.IsFailure)
                return Result<HiveDto>.Failure(apiaryIdResult.Error!.Message, ErrorType.Validation);

            // Check apiary exists and belongs to current user
            var apiary = await apiaryRepository.GetByIdAsync(apiaryIdResult.Value, ct);
            if (apiary is null)
                return Result<HiveDto>.Failure("Apiary not found.", ErrorType.NotFound);

            if (!apiary.IsOwnedBy(currentUserService.UserId))
                return Result<HiveDto>.Failure("Access denied.", ErrorType.Unauthorized);

            var hiveResult = Hive.Create(
                request.ApiaryId,
                request.Label,
                request.Type,
                request.BoxColor,
                request.QueenAge,
                request.Note,
                dateTimeProvider.UtcNow);

            if (hiveResult.IsFailure)
                return Result<HiveDto>.Failure(hiveResult.Error!.Message, ErrorType.Validation);

            var hive = hiveResult.Value;

            try
            {
                await hiveRepository.AddAsync(hive, ct);

                logger.LogInformation("[HIVE] Hive '{Label}' (ID: {Id}) added to apiary {ApiaryId}.",
                    hive.Label, hive.Id.Value, request.ApiaryId);

                return Result<HiveDto>.Success(mapper.Map(hive));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[HIVE] Error adding hive to apiary {ApiaryId}.", request.ApiaryId);
                return Result<HiveDto>.Failure("Failed to add hive.", ErrorType.Failure);
            }
        }
    }
}
