using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.Apiaries.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Apiaries.Queries.GetMyApiaries
{
    // QUERY
    public record GetMyApiariesQuery : IRequest<Result<IReadOnlyCollection<ApiaryDto>>>;

    // HANDLER
    internal class GetMyApiariesHandler(
        IApiaryRepository apiaryRepository,
        ICurrentUserService currentUserService,
        IMapper<Apiary, ApiaryDto> mapper,
        ILogger<GetMyApiariesHandler> logger) : IRequestHandler<GetMyApiariesQuery, Result<IReadOnlyCollection<ApiaryDto>>>
    {
        public async Task<Result<IReadOnlyCollection<ApiaryDto>>> Handle(GetMyApiariesQuery request, CancellationToken ct)
        {
            try
            {
                var apiaries = await apiaryRepository.GetAllByOwnerAsync(currentUserService.UserId, ct);

                var dtos = apiaries.Select(mapper.Map).ToList().AsReadOnly();

                return Result<IReadOnlyCollection<ApiaryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[APIARY] Error fetching apiaries for user {OwnerId}.", currentUserService.UserId);
                return Result<IReadOnlyCollection<ApiaryDto>>.Failure("Failed to fetch apiaries.", ErrorType.Failure);
            }
        }
    }
}
