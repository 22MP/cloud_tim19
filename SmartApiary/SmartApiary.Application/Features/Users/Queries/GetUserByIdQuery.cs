using MediatR;
using SmartApiary.Application.Features.Users.DTOs;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Application.Features.Users.Queries;

public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto>>;

internal class GetUserByIdHandler(
    IUserRepository userRepository,
    IMapper<User, UserDto> mapper) : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var idResult = EntityId.Create(request.UserId);
        if (idResult.IsFailure)
            return Result<UserDto>.Failure(idResult.Error!.Message, ErrorType.Validation);

        var user = await userRepository.GetByIdAsync(idResult.Value, ct);
        if (user is null)
            return Result<UserDto>.Failure("User not found.", ErrorType.NotFound);

        return Result<UserDto>.Success(mapper.Map(user));
    }
}