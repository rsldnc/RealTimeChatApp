using ChatServiceV2.Context;
using ChatServiceV2.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatServiceV2.Hubs
{
    public sealed class ChatHub(ApplicationDbContext context) : Hub
    {
        public static Dictionary<string, Guid> Users = new Dictionary<string, Guid>();
        public async Task Connect(Guid userId)
        {
            Users[Context.ConnectionId] = userId;

            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Status = "online";
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Users.TryGetValue(Context.ConnectionId, out var userId))
            {
                Users.Remove(Context.ConnectionId);

                var user = await context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Status = "offline";
                    await context.SaveChangesAsync();

                    await Clients.All.SendAsync("Users", user);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
