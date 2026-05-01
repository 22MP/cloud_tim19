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

namespace SmartApiary.Application.Features.Hives.Queries.GetHiveById
{
    // QUERY
    public record GetHiveByIdQuery(string HiveId) : IRequest<Result<HiveDto>>;

    // VALIDATOR
    public class GetHiveByIdValidator : AbstractValidator<GetHiveByIdQuery>
    {
        public GetHiveByIdValidator()
        {
            RuleFor(x => x.HiveId)
                .NotEmpty().WithMessage("Hive ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid hive ID format.");
        }
    }

    // HANDLER
    internal class GetHiveByIdHandler(
        IApiaryRepository apiaryRepository,
        IHiveRepository hiveRepository,
        ICurrentUserService currentUserService,
        IMapper<Hive, HiveDto> mapper,
        ILogger<GetHiveByIdHandler> logger) : IRequestHandler<GetHiveByIdQuery, Result<HiveDto>>
    {
        public async Task<Result<HiveDto>> Handle(GetHiveByIdQuery request, CancellationToken ct)
        {
            var idResult = EntityId.Create(request.HiveId);
            if (idResult.IsFailure)
                return Result<HiveDto>.Failure(idResult.Error!.Message, ErrorType.Validation);

            try
            {
                var hive = await hiveRepository.GetByIdAsync(idResult.Value, ct);
                if (hive is null)
                    return Result<HiveDto>.Failure("Hive not found.", ErrorType.NotFound);

                var apiary = await apiaryRepository.GetByIdAsync(hive.ApiaryId, ct);
                if (apiary is null || !apiary.IsOwnedBy(currentUserService.UserId))
                    return Result<HiveDto>.Failure("Access denied.", ErrorType.Unauthorized);

                return Result<HiveDto>.Success(mapper.Map(hive));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[HIVE] Error fetching hive {Id}.", request.HiveId);
                return Result<HiveDto>.Failure("Failed to fetch hive.", ErrorType.Failure);
            }
        }
    }
}
