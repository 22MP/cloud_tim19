using MediatR;
using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;
using SmartApiary.Application.Interfaces.Repositories;

namespace SmartApiary.Application.Features.Users.Queries;


public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAt);

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