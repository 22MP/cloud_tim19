using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;

namespace SmartApiary.Application.Features.Users.Commands.Login;

internal class LoginHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    IDateTimeProvider dateTimeProvider,
    ILogger<LoginHandler> logger) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
            return Result<LoginResponse>.Failure(
                "Invalid email or password.", ErrorType.Unauthorized);

        if (!user.CanLogin)
            return Result<LoginResponse>.Failure(
                "Account is not active.", ErrorType.Unauthorized);

        var isValid = passwordHasher.Verify(request.Password, user.PasswordHash!);

        if (!isValid)
            return Result<LoginResponse>.Failure(
                "Invalid email or password.", ErrorType.Unauthorized);

        var token = jwtService.GenerateToken(
            user.Id.Value, user.Email, user.Role.ToString());

        logger.LogInformation("[USER] User {Email} logged in.", user.Email);

        return Result<LoginResponse>.Success(new LoginResponse(
            token,
            user.Id.Value,
            user.Email,
            user.Role.ToString()));
    }
}