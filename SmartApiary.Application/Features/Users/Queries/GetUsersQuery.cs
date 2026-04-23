using MediatR;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Features.Users.DTOs;

namespace SmartApiary.Application.Features.Users.Queries;

public record GetUsersQuery() : IRequest<Result<IEnumerable<UserDto>>>;

// HANDLER
internal class GetUsersHandler(
    IUserRepository userRepository,
    IMapper<User, UserDto> mapper) : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserDto>>>
{
    public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await userRepository.GetAllAsync(ct);
        var dtos = users.Select(mapper.Map).ToList();
        return Result<IEnumerable<UserDto>>.Success(dtos);
    }
}