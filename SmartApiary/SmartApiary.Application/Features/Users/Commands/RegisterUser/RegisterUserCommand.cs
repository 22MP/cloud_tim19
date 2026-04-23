using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums.Users;

namespace SmartApiary.Application.Features.Users.Commands;

public record RegisterUserCommand : IRequest<Result<string>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public UserRole Role { get; init; }
}