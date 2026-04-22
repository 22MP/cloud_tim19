using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Users.Commands;

internal class RegisterUserHandler(
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider,
    ILogger<RegisterUserHandler> logger) : IRequestHandler<RegisterUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var now = dateTimeProvider.UtcNow;

        
        var userResult = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Role,
            now
        );

        if (userResult.IsFailure)
        {
            return Result<string>.Failure(userResult.Error!.Message, ErrorType.Validation);
        }

        var user = userResult.Value;

        try
        {
            
            var existingUser = await userRepository.GetByEmailAsync(user.Email, ct);
            if (existingUser is not null)
            {
                return Result<string>.Failure("User with this email already exists.", ErrorType.Conflict);
            }

            
            await userRepository.SaveAsync(user, ct);

            logger.LogInformation("[USER] New user {Email} registered by Admin. Role: {Role}",
                user.Email, user.Role);

           
            return Result<string>.Success(user.Id.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Registration failed for {Email}. Reason: {Message}", request.Email, ex.Message);
            return Result<string>.Failure($"Infrastructure error: {ex.Message}", ErrorType.Failure);
        }
    }
}