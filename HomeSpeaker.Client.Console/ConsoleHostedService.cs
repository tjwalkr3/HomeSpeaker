using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace HomeSpeaker.Client.Console
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly HomeSpeakerClient homeSpeakerClient;
        private readonly IConfiguration config;
        private readonly IHostApplicationLifetime _appLifetime;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            HomeSpeakerClient homeSpeakerClient,
            IConfiguration config)
        {
            _logger = logger;
            this.homeSpeakerClient = homeSpeakerClient ?? throw new ArgumentNullException(nameof(homeSpeakerClient));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Hello World!");

                        // Simulate real work is being done
                        await Task.Delay(1000);

                        WriteLine("What greeting would you like to send?");
                        var myValue = ReadLine();
                        var response = homeSpeakerClient.DoGreeting(myValue);
                        WriteLine($"Response from server: {response}");

                        _logger.LogInformation("Asking server for songs...");
                        var client2 = new HomeSpeaker.Server.HomeSpeaker.HomeSpeakerClient(GrpcChannel.ForAddress(config["HomeSpeaker.Server"]));
                        var songsResponse = client2.GetSongs(new Server.GetSongsRequest { });
                        await foreach(var song in songsResponse.ResponseStream.ReadAllAsync())
                        {
                            WriteLine(song);
                        }
                        _logger.LogInformation("All done getting songs");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
