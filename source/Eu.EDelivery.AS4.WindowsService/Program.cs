using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;


namespace Eu.EDelivery.AS4.WindowsService
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = ".NET Joke Service";
                })
                .ConfigureServices(services =>
                {
                    LoggerProviderOptions.RegisterProviderOptions<
                        EventLogSettings, EventLogLoggerProvider>(services);

                    services.AddSingleton<JokeService>();
                    services.AddHostedService<WindowsBackgroundService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}

// https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service
