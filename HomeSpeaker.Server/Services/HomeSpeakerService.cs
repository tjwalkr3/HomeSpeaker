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
            var songs = library.Songs.Select(s => new SongMessage
            {
                Album = s.Album,
                Artist = s.Artist,
                Name = s.Name,
                Path = s.Path,
                SongId = s.SongId
            });
            var reply = new GetSongsReply();
            reply.Songs.AddRange(songs);
            await responseStream.WriteAsync(reply);
        }

        public override Task<PlaySongReply> PlaySong(PlaySongRequest request, ServerCallContext context)
        {
            var song = library.Songs.FirstOrDefault(s => s.SongId == request.SongId);
            var reply = new PlaySongReply { Ok = false };
            if (song != null)
            {
                musicPlayer.PlaySong(song.Path);
                reply.Ok = true;
            }
            return Task.FromResult(reply);
        }

        public override Task<Empty> ResetLibrary(Empty request, ServerCallContext context)
        {
            library.ResetLibrary();
            return Task.FromResult(new Empty());
        }
    }
}
