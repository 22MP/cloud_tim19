using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Users.DTOs
{
    public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAt);
}
