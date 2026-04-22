using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Features.Users.Queries;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Users.Mappers;

internal sealed class UserMapper : IMapper<User, UserDto>
{
    public UserDto Map(User source)
    {
        return new UserDto(
            source.Id.Value,
            source.FirstName,
            source.LastName,
            source.Email,
            source.Role.ToString(),
            source.Status.ToString(),
            source.CreatedAt);
    }
}