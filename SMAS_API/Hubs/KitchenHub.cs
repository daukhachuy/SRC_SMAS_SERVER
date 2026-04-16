using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SMAS_API.Hubs
{
    [Authorize(Roles = "Kitchen,Waiter,Manager,Admin")]
    public class KitchenHub : Hub
    {
        public async Task JoinKitchen()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Kitchen");
        }

        public async Task JoinWaiter()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Waiter");
        }

        public override async Task OnConnectedAsync()
        {
            var role = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
