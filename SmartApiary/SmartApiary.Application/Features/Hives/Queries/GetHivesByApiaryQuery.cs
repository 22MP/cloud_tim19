using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.Hives.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Hives.Queries.GetHivesByApiary
{
    // QUERY
    public record GetHivesByApiaryQuery(string ApiaryId) : IRequest<Result<IReadOnlyCollection<HiveDto>>>;

    // VALIDATOR
    public class GetHivesByApiaryValidator : AbstractValidator<GetHivesByApiaryQuery>
    {
        public GetHivesByApiaryValidator()
        {
            RuleFor(x => x.ApiaryId)
                .NotEmpty().WithMessage("Apiary ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid apiary ID format.");
        }
    }

    // HANDLER
    internal class GetHivesByApiaryHandler(
        IApiaryRepository apiaryRepository,
        IHiveRepository hiveRepository,
        ICurrentUserService currentUserService,
        IMapper<Hive, HiveDto> mapper,
        ILogger<GetHivesByApiaryHandler> logger) : IRequestHandler<GetHivesByApiaryQuery, Result<IReadOnlyCollection<HiveDto>>>
    {
        public async Task<Result<IReadOnlyCollection<HiveDto>>> Handle(GetHivesByApiaryQuery request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.ApiaryId);
            if (idResult.IsFailure)
                return Result<IReadOnlyCollection<HiveDto>>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var apiary = await apiaryRepository.GetByIdAsync(idResult.Value, ct);
            if (apiary is null)
                return Result<IReadOnlyCollection<HiveDto>>.Failure("Apiary not found.", ErrorType.NotFound);

            if (!apiary.IsOwnedBy(currentUserService.UserId))
                return Result<IReadOnlyCollection<HiveDto>>.Failure("Access denied.", ErrorType.Unauthorized);

            try
            {
                var hives = await hiveRepository.GetAllByApiaryAsync(idResult.Value, ct);
                var dtos = hives.Select(mapper.Map).ToList().AsReadOnly();
                return Result<IReadOnlyCollection<HiveDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[HIVE] Error fetching hives for apiary {ApiaryId}.", request.ApiaryId);
                return Result<IReadOnlyCollection<HiveDto>>.Failure("Failed to fetch hives.", ErrorType.Failure);
            }
        }
    }
}
