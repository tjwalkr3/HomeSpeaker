using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Server
{
    public class HomeSpeakerService : HomeSpeaker.HomeSpeakerBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly Mp3Library library;

        public HomeSpeakerService(ILogger<GreeterService> logger, Mp3Library library)
        {
            _logger = logger;
            this.library = library;
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
    }
}
