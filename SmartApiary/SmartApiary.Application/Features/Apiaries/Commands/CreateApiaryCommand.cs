using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.Apiaries.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Apiaries.Commands.CreateApiary
{
    // COMMAND
    public record CreateApiaryCommand : IRequest<Result<ApiaryDto>>
    {
        public string Name { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string? Description { get; init; }
    }

    // VALIDATOR
    public class CreateApiaryValidator : AbstractValidator<CreateApiaryCommand>
    {
        public CreateApiaryValidator()
        {
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
    internal class CreateApiaryHandler(
        IApiaryRepository apiaryRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IMapper<Apiary, ApiaryDto> mapper,
        ILogger<CreateApiaryHandler> logger) : IRequestHandler<CreateApiaryCommand, Result<ApiaryDto>>
    {
        public async Task<Result<ApiaryDto>> Handle(CreateApiaryCommand request, CancellationToken ct)
        {
            var now = dateTimeProvider.UtcNow;
            var ownerId = currentUserService.UserId;

            var apiaryResult = Apiary.Create(
                ownerId,
                request.Name,
                request.Latitude,
                request.Longitude,
                request.Description,
                now);

            if (apiaryResult.IsFailure)
                return Result<ApiaryDto>.Failure(apiaryResult.Error!.Message, ErrorType.Validation);

            var apiary = apiaryResult.Value;

            try
            {
                await apiaryRepository.AddAsync(apiary, ct);

                logger.LogInformation("[APIARY] New apiary '{Name}' (ID: {Id}) created by user {OwnerId}.",
                    apiary.Name, apiary.Id.Value, ownerId);

                return Result<ApiaryDto>.Success(mapper.Map(apiary));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[APIARY] Error creating apiary '{Name}' for user {OwnerId}.", request.Name, ownerId);
                return Result<ApiaryDto>.Failure("Failed to create apiary.", ErrorType.Failure);
            }
        }
    }
}
