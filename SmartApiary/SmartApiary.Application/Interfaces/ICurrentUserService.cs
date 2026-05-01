namespace SmartApiary.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserRole { get; }
    }
}
