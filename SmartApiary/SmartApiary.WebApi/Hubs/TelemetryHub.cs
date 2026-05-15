using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SmartApiary.WebApi.Hubs;

[Authorize]
public sealed class TelemetryHub : Hub
{
    public Task JoinApiaryGroup(string apiaryId)
    {
        if (string.IsNullOrWhiteSpace(apiaryId))
            throw new HubException("Apiary id is required.");

        return Groups.AddToGroupAsync(Context.ConnectionId, GetApiaryGroupName(apiaryId));
    }

    public Task LeaveApiaryGroup(string apiaryId)
    {
        if (string.IsNullOrWhiteSpace(apiaryId))
            return Task.CompletedTask;

        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetApiaryGroupName(apiaryId));
    }

    public static string GetApiaryGroupName(string apiaryId) => $"apiary:{apiaryId}";
}
