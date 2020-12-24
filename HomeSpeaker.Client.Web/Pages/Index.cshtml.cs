using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using HomeSpeaker.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HomeSpeaker.Client;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HomeSpeakerClient homeSpeakerClient;

        public IndexModel(ILogger<IndexModel> logger, HomeSpeakerClient homeSpeakerClient)
        {
            this._logger = logger;
            this.homeSpeakerClient = homeSpeakerClient;
        }

        private List<Song> songs = new List<Song>();
        public IEnumerable<Song> Songs => songs;
        public IEnumerable<Song> Queue { get; private set; }
        public PlayerStatus Status { get; private set; }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Getting songs...");
            await getSongs();
            _logger.LogInformation($"Found {songs.Count} songs");
        }

        private async Task getSongs()
        {
            var getSongsReply = homeSpeakerClient.GetSongs(new Server.gRPC.GetSongsRequest { });
            await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
            {
                songs.AddRange(reply.Songs.Select(s => s.ToSong()));
            }

            var getQueueReply = homeSpeakerClient.GetPlayQueue(new Server.gRPC.GetSongsRequest { });
            await foreach(var reply in getQueueReply.ResponseStream.ReadAllAsync())
            {
                Queue = from songMessage in reply.Songs
                        select songMessage.ToSong();
            }
        }

        public async Task<IActionResult> OnGetPlaySongAsync(int songId)
        {
            _logger.LogInformation($"User requested to play song {songId}");
            await homeSpeakerClient.PlaySongAsync(new Server.gRPC.PlaySongRequest { SongId = songId });
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPlayAlbumAsync(string artist, string album)
        {
            _logger.LogInformation($"Queuing {artist} | {album}");
            await getSongs();
            foreach (var song in songs.Where(s => s.Artist == artist && s.Album == album))
            {
                await homeSpeakerClient.EnqueueSongAsync(new Server.gRPC.PlaySongRequest { SongId = song.SongId });
            }
            return RedirectToPage();
        }
    }
}
