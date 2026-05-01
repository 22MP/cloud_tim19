using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces;
using SmartApiary.Infrastructure.Services;

namespace SmartApiary.Infrastructure.Extensions;

internal static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IImageResizer, ImageResizerService>();

        services.AddHttpContextAccessor();
        return services;
    }
}