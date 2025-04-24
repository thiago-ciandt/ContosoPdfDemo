using Microsoft.AspNetCore.SignalR;

namespace Contoso.WebApi.Hubs
{
    public class AdminHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Admin connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }
    }
}
