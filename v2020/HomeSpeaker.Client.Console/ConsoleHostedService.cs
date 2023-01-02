using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace HomeSpeaker.Client.Console
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration config;
        private readonly IHostApplicationLifetime _appLifetime;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IConfiguration config)
        {
            _logger = logger;
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
                        _logger.LogInformation("gRPC Console Client!");
                        var httpHandler = new HttpClientHandler();
                        // Return `true` to allow certificates that are untrusted/invalid
                        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                        var channel = GrpcChannel.ForAddress(config["HomeSpeaker.Server"], new GrpcChannelOptions { HttpHandler = httpHandler });
                        var client2 = new HomeSpeaker.Server.gRPC.HomeSpeaker.HomeSpeakerClient(channel);

                        if (config["SongID"] != null)
                        {
                            var songId = int.Parse(config["SongID"]);
                            client2.PlaySong(new Server.gRPC.PlaySongRequest { SongId = songId });
                            return;
                        }

                        _logger.LogInformation("Asking server for songs...");
                        var songsResponse = client2.GetSongs(new Server.gRPC.GetSongsRequest { });
                        await foreach (var reply in songsResponse.ResponseStream.ReadAllAsync())
                        {
                            foreach (var song in reply.Songs)
                            {
                                WriteLine($"{song.SongId,4}: {song.Name}");
                            }
                        }
                        _logger.LogInformation("All done getting songs");

                        WriteLine("What song id would you like to play?");
                        var songToPlay = int.Parse(ReadLine());
                        client2.PlaySong(new Server.gRPC.PlaySongRequest { SongId = songToPlay });
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
