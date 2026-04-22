namespace SmartApiary.Infrastructure.Persistence.AzureTable.Entities;

internal class UserEntity : BaseTableEntity
{
    public string Id { get; set; } = default!; 
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public string? ActivationToken { get; set; }
    public DateTime? ActivationTokenExpiry { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
}