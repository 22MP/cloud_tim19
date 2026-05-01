using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.Apiaries.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Apiaries.Commands.UpdateApiary
{
    // COMMAND
    public record UpdateApiaryCommand : IRequest<Result<ApiaryDto>>
    {
        public string ApiaryId { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string? Description { get; init; }
    }

    // VALIDATOR
    public class UpdateApiaryValidator : AbstractValidator<UpdateApiaryCommand>
    {
        public UpdateApiaryValidator()
        {
            RuleFor(x => x.ApiaryId)
                .NotEmpty().WithMessage("Apiary ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid apiary ID format.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Apiary name is required.")
                .MaximumLength(100).WithMessage("Apiary name must not exceed 100 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => x.Description is not null);
        }
    }

    // HANDLER
    internal class UpdateApiaryHandler(
        IApiaryRepository apiaryRepository,
        ICurrentUserService currentUserService,
        IMapper<Apiary, ApiaryDto> mapper,
        ILogger<UpdateApiaryHandler> logger) : IRequestHandler<UpdateApiaryCommand, Result<ApiaryDto>>
    {
        public async Task<Result<ApiaryDto>> Handle(UpdateApiaryCommand request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.ApiaryId);
            if (idResult.IsFailure)
                return Result<ApiaryDto>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var apiary = await apiaryRepository.GetByIdAsync(idResult.Value, ct);
            if (apiary is null)
                return Result<ApiaryDto>.Failure("Apiary not found.", ErrorType.NotFound);

            if (!apiary.IsOwnedBy(currentUserService.UserId))
                return Result<ApiaryDto>.Failure("Access denied.", ErrorType.Unauthorized);

            var updateResult = apiary.UpdateInfo(request.Name, request.Latitude, request.Longitude, request.Description);
            if (updateResult.IsFailure)
                return Result<ApiaryDto>.Failure(updateResult.Error!.Message, ErrorType.Validation);

            try
            {
                await apiaryRepository.UpdateAsync(apiary, ct);

                logger.LogInformation("[APIARY] Apiary {Id} updated by user {OwnerId}.", apiary.Id.Value, currentUserService.UserId);

                return Result<ApiaryDto>.Success(mapper.Map(apiary));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[APIARY] Error updating apiary {Id}.", request.ApiaryId);
                return Result<ApiaryDto>.Failure("Failed to update apiary.", ErrorType.Failure);
            }
        }
    }
}
