using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces;
using System.Security.Claims;

namespace SmartApiary.Infrastructure.Services;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string UserId =>
    httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
    ?? throw new UnauthorizedAccessException("User ID not found in token.");

    public string UserRole =>
        httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Role)?.Value
        ?? throw new UnauthorizedAccessException("User role not found.");
}