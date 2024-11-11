using Microsoft.AspNetCore.SignalR;

namespace LearnSystem.SignalR
{
    public class LearnSystemSignalRHub : Hub<ISignalHubClient>
    {
        public static string? ConnectionId { get; private set; }

        public override Task OnConnectedAsync()
        {
            ConnectionId = Context.ConnectionId;

            Clients.Client(ConnectionId).SendServerTimeAsync(DateTime.Now);

            return base.OnConnectedAsync();
        }

    }

    public interface ISignalHubClient
    {
        public Task SendServerTimeAsync(DateTime date);
    }
}
