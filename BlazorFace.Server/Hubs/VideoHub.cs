using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace BlazorFace.Server.Hubs;

public class VideoHub : Hub
{
    public ILogger<VideoHub> Logger { get; set; }

    public VideoHub(ILogger<VideoHub> logger)
    {
        Logger = logger;
    }

    public async Task JoinRoom(Guid roomId)
    {
        Logger.LogInformation($"User: {Context.ConnectionId} Joined Room: {roomId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).SendAsync("userConnected", $"{Context.ConnectionId} has joined the room {roomId}", roomId);
    }

    public async Task LeaveRoom(Guid roomId)
    {
        Logger.LogInformation($"User: {Context.ConnectionId} Left Room: {roomId}");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Group(roomId.ToString()).SendAsync("userDisconnected", $"{Context.ConnectionId} has left the room {roomId}", roomId);
    }
}
