using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands.ActivateAccount;

public record ActivateAccountCommand : IRequest<Result>
{
    public string Token { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}