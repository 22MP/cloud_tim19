using SmartApiary.Application.Interfaces;
using SmartApiary.Domain.Enums.Users;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Mappers;

internal sealed class UserTableMapper : ITableMapper<User, UserEntity>
{
    public UserEntity ToEntity(User domain)
    {
        return new UserEntity
        {
            PartitionKey = "Users",
            RowKey = domain.Email.ToLower().Trim(),

            Id = domain.Id.Value, 
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Role = domain.Role.ToString(),
            Status = domain.Status.ToString(),
            PhoneNumber = domain.PhoneNumber,
            PasswordHash = domain.PasswordHash,
            ActivationToken = domain.ActivationToken,
            ActivationTokenExpiry = domain.ActivationTokenExpiry,
            PasswordResetToken = domain.PasswordResetToken,
            PasswordResetTokenExpiry = domain.PasswordResetTokenExpiry,
            CreatedAt = domain.CreatedAt
        };
    }

    public User? ToDomain(UserEntity entity)
    {
        if (!Enum.TryParse<UserRole>(entity.Role, out var role)) role = UserRole.Beekeeper;
        if (!Enum.TryParse<UserStatus>(entity.Status, out var status)) status = UserStatus.Pending;

        
        var userResult = User.Load(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.RowKey,
            entity.PhoneNumber,
            entity.PasswordHash,
            role,
            status,
            entity.ActivationToken,
            entity.ActivationTokenExpiry,
            entity.PasswordResetToken,
            entity.PasswordResetTokenExpiry,
            entity.CreatedAt
        );

        if (userResult.IsFailure)
        {
            
            return null;
        }

        return userResult.Value;
    }
}