using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HomeSpeaker.Server.gRPC;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Server
{
    public class HomeSpeakerService : gRPC.HomeSpeaker.HomeSpeakerBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly Mp3Library library;
        private readonly IMusicPlayer musicPlayer;

        public HomeSpeakerService(ILogger<GreeterService> logger, Mp3Library library, IMusicPlayer musicPlayer)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.library = library ?? throw new System.ArgumentNullException(nameof(library));
            this.musicPlayer = musicPlayer ?? throw new System.ArgumentNullException(nameof(musicPlayer));
        }

        public override async Task GetSongs(GetSongsRequest request, IServerStreamWriter<GetSongsReply> responseStream, ServerCallContext context)
        {
            var reply = new GetSongsReply();
            if (library?.Songs?.Any() ?? false)
            {
                _logger.LogInformation("Found songs!  Sending to client.");
                var songs = library.Songs.Select(s => new SongMessage
                {
                    Album = s.Album,
                    Artist = s.Artist,
                    Name = s.Name ?? "[ No Name ]",
                    Path = s.Path,
                    SongId = s.SongId
                });
                reply.Songs.AddRange(songs);
            }
            else
            {
                _logger.LogInformation("No songs found.  Sending back empty list.");
            }
            await responseStream.WriteAsync(reply);
        }

        public override Task<PlaySongReply> PlaySong(PlaySongRequest request, ServerCallContext context)
        {
            var song = library.Songs.FirstOrDefault(s => s.SongId == request.SongId);
            var reply = new PlaySongReply { Ok = false };
            if (song != null)
            {
                Task.Run(() =>
                    musicPlayer.PlaySong(song.Path)
                );
                reply.Ok = true;
            }
            return Task.FromResult(reply);
        }

        public override Task<Empty> ResetLibrary(Empty request, ServerCallContext context)
        {
            library.ResetLibrary();
            return Task.FromResult(new Empty());
        }

        public override Task<PlaySongReply> EnqueueSong(PlaySongRequest request, ServerCallContext context)
        {
            var song = library.Songs.FirstOrDefault(s => s.SongId == request.SongId);
            var reply = new PlaySongReply { Ok = false };
            if (song != null)
            {
                Task.Run(() =>
                    musicPlayer.EnqueueSong(song.Path)
                );
                reply.Ok = true;
            }
            return Task.FromResult(reply);
        }

        public override Task<GetStatusReply> GetPlayerStatus(GetStatusRequest request, ServerCallContext context)
        {
            var status = musicPlayer.Status;
            return Task.FromResult(new GetStatusReply
            {
                Elapsed = Duration.FromTimeSpan(status.Elapsed),
                PercentComplete = (double)status.PercentComplete,
                Remaining = Duration.FromTimeSpan(status.Remaining),
                StilPlaying = status.StillPlaying
            });
        }

        public override async Task GetPlayQueue(GetSongsRequest request, IServerStreamWriter<GetSongsReply> responseStream, ServerCallContext context)
        {
            var reply = new GetSongsReply();
            System.Collections.Generic.IEnumerable<Shared.Song> songQueue = musicPlayer.SongQueue;
            if (songQueue.Any())
            {
                _logger.LogInformation("Found songs in queue!  Sending to client.");
                var songs = songQueue.Select(s => new SongMessage
                {
                    Album = s.Album,
                    Artist = s.Artist,
                    Name = s.Name ?? "[ No Name ]",
                    Path = s.Path,
                    SongId = s.SongId
                });
                reply.Songs.AddRange(songs);
            }
            else
            {
                _logger.LogInformation("No songs in queue.  Sending back empty list.");
            }
            await responseStream.WriteAsync(reply);
        }
    }
}
