using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace HomeSpeaker.Client.Console
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ConsoleHostedService>();
                    services.AddSingleton<HomeSpeakerClient>();
                })
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .RunConsoleAsync();
        }
    }
}
