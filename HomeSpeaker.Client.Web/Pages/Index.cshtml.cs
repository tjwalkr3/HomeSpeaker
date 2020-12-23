using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using HomeSpeaker.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using static HomeSpeaker.Server.HomeSpeaker;

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

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Getting songs...");
            var getSongsReply = homeSpeakerClient.GetSongs(new Server.GetSongsRequest { });
            await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
            {
                foreach (var song in reply.Songs)
                {
                    songs.Add(new Song(song.SongId, song.Name, song.Album, song.Artist, song.Path));
                }
            }
            _logger.LogInformation($"Found {songs.Count} songs");
        }

        public async Task<IActionResult> OnGetPlaySongAsync(int songId)
        {
            _logger.LogInformation($"User requested to play song {songId}");
            await homeSpeakerClient.PlaySongAsync(new Server.PlaySongRequest { SongId = songId });
            return RedirectToPage();
        }
    }
}
