using System;
using System.IO;
using System.Threading;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AS4.ParserService.Services;


namespace AS4.ParserService
{
    /// <summary>
    /// Start for the Payload Service Web API.
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Entry method for the Payload Service Web API.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Start(CancellationToken.None);
        }

        /// <summary>
        /// Starts the PayloadService with a cancellation-token
        /// This method is used when the service is started in process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void Start(CancellationToken cancellationToken)
        {
            var hostBuilder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("./bin/appsettings.parserservice.json", true)
                .AddJsonFile("./appsettings.parserservice.json", true)
                .AddEnvironmentVariables()
                .Build();

            var url = config.GetValue<string>("Url") ?? "http://localhost:3000";

            var host = hostBuilder
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();

            host.ConfigureServices(services =>
                {
                    services.AddSingleton(_ => new DecodeService());
                    services.AddSingleton(_ => new EncodeService());
                }
            );
            
            host.UseUrls(url)
                .Build()
                .RunAsync(cancellationToken)
                .Wait(cancellationToken);

            Console.WriteLine("Parser Service shutdown");
        }
    }
}