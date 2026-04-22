using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Enums.Users;
using SmartApiary.Domain.Events;
using SmartApiary.Domain.ValueObjects;

namespace SmartApiary.Domain.Models;

// AGGREGATE ROOT
public class User : AggregateRoot
{
    public EntityId Id { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }

    public string? ActivationToken { get; private set; }
    public DateTime? ActivationTokenExpiry { get; private set; }

    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private User() { }

    private User(
        EntityId id,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        UserRole role,
        UserStatus status,
        DateTime createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
        Status = status;
        CreatedAt = createdAt;
    }

    #region Factory Methods

    public static Result<User> Create(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        UserRole role,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<User>.Failure("First name is required.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<User>.Failure("Last name is required.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Result<User>.Failure("Invalid email address.", ErrorType.Validation);

        var user = new User(
            EntityId.New(),
            firstName.Trim(),
            lastName.Trim(),
            email.Trim().ToLowerInvariant(),
            phoneNumber?.Trim(),
            role,
            UserStatus.Pending,
            createdAt
        );

        user.GenerateActivationToken(createdAt);

        user.AddDomainEvent(new UserCreatedEvent(user.Id.Value, user.Email, user.FirstName));

        return Result<User>.Success(user);
    }

    public static Result<User> Load(
        string id,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        string? passwordHash,
        UserRole role,
        UserStatus status,
        string? activationToken,
        DateTime? activationTokenExpiry,
        string? passwordResetToken,
        DateTime? passwordResetTokenExpiry,
        DateTime createdAt)
    {
        var idResult = EntityId.Create(id);

        if (idResult.IsFailure)
            return Result<User>.Failure(idResult.Error!.Message, ErrorType.Validation);

        return Result<User>.Success(new User(
            idResult.Value,
            firstName,
            lastName,
            email.Trim().ToLowerInvariant(),
            phoneNumber,
            role,
            status,
            createdAt
        )
        {
            PasswordHash = passwordHash,
            ActivationToken = activationToken,
            ActivationTokenExpiry = activationTokenExpiry,
            PasswordResetToken = passwordResetToken,
            PasswordResetTokenExpiry = passwordResetTokenExpiry
        });
    }

    #endregion

    #region Domain Logic

    public void GenerateActivationToken(DateTime now)
    {
        ActivationToken = Guid.NewGuid().ToString();
        ActivationTokenExpiry = now.AddHours(24);
    }

    public Result ActivateAccount(string passwordHash, string token, DateTime now)
    {
        if (Status == UserStatus.Active)
            return Result.Failure("Account is already active.", ErrorType.Conflict);

        if (ActivationToken is null || ActivationToken != token)
            return Result.Failure("Invalid activation token.", ErrorType.Validation);

        if (ActivationTokenExpiry.HasValue && ActivationTokenExpiry.Value < now)
            return Result.Failure("Activation token has expired.", ErrorType.Validation);

        PasswordHash = passwordHash;
        Status = UserStatus.Active;
        ActivationToken = null;
        ActivationTokenExpiry = null;

        AddDomainEvent(new UserActivatedEvent(Id.Value, Email));

        return Result.Success();
    }

    public void GeneratePasswordResetToken(DateTime now)
    {
        PasswordResetToken = Guid.NewGuid().ToString();
        PasswordResetTokenExpiry = now.AddHours(1);
    }

    public Result ResetPassword(string newPasswordHash, string token, DateTime now)
    {
        if (PasswordResetToken is null || PasswordResetToken != token)
            return Result.Failure("Invalid reset token.", ErrorType.Validation);

        if (PasswordResetTokenExpiry.HasValue && PasswordResetTokenExpiry.Value < now)
            return Result.Failure("Reset token has expired.", ErrorType.Validation);

        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;

        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status == UserStatus.Suspended)
            return Result.Failure("Account is already suspended.", ErrorType.Conflict);

        Status = UserStatus.Suspended;
        return Result.Success();
    }

    public Result Reactivate()
    {
        if (Status == UserStatus.Active)
            return Result.Failure("Account is already active.", ErrorType.Conflict);

        Status = UserStatus.Active;
        return Result.Success();
    }

    #endregion

    #region Helpers

    public string FullName => $"{FirstName} {LastName}";

    public bool HasPassword => !string.IsNullOrEmpty(PasswordHash);

    public bool CanLogin => Status == UserStatus.Active && HasPassword;

    #endregion
}