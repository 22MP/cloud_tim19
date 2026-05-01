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

namespace SmartApiary.Application.Features.Apiaries.Queries.GetApiaryById
{
    // QUERY
    public record GetApiaryByIdQuery(string ApiaryId) : IRequest<Result<ApiaryDto>>;

    // VALIDATOR
    public class GetApiaryByIdValidator : AbstractValidator<GetApiaryByIdQuery>
    {
        public GetApiaryByIdValidator()
        {
            RuleFor(x => x.ApiaryId)
                .NotEmpty().WithMessage("Apiary ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid apiary ID format.");
        }
    }

    // HANDLER
    internal class GetApiaryByIdHandler(
        IApiaryRepository apiaryRepository,
        ICurrentUserService currentUserService,
        IMapper<Apiary, ApiaryDto> mapper,
        ILogger<GetApiaryByIdHandler> logger) : IRequestHandler<GetApiaryByIdQuery, Result<ApiaryDto>>
    {
        public async Task<Result<ApiaryDto>> Handle(GetApiaryByIdQuery request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.ApiaryId);
            if (idResult.IsFailure)
                return Result<ApiaryDto>.Failure(idResult.Error!.Message, ErrorType.Validation);

            try
            {
                var apiary = await apiaryRepository.GetByIdWithHivesAsync(idResult.Value, ct);

                if (apiary is null)
                    return Result<ApiaryDto>.Failure("Apiary not found.", ErrorType.NotFound);

                if (!apiary.IsOwnedBy(currentUserService.UserId))
                    return Result<ApiaryDto>.Failure("Access denied.", ErrorType.Unauthorized);

                return Result<ApiaryDto>.Success(mapper.Map(apiary));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[APIARY] Error fetching apiary {Id}.", request.ApiaryId);
                return Result<ApiaryDto>.Failure("Failed to fetch apiary.", ErrorType.Failure);
            }
        }
    }
}
