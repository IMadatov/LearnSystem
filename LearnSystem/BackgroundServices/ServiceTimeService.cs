
using LearnSystem.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace LearnSystem.BackgroundServices
{
    public class ServiceTimeService(IServiceProvider sp) : BackgroundService
    {
        public async Task DoAsync(CancellationToken cancellationToken)
        {
            using var scope = sp.CreateScope();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<LearnSystemSignalRHub, ISignalHubClient>>();

            while (true)
            {
                _ = hubContext.Clients.All.SendServerTimeAsync(DateTime.Now);

               
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return DoAsync(stoppingToken);
        }
    }
}
