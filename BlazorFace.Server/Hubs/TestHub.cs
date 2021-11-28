using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFace.Server.Hubs;

public class TestHub : Hub
{

    public async Task TestSendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

}

