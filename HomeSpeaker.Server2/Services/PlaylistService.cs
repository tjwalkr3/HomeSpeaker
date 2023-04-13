using HomeSpeaker.Server;
using HomeSpeaker.Server2.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeSpeaker.Server2.Services;

public class PlaylistService
{
    private readonly MusicContext dbContext;
    private readonly Mp3Library mp3Library;
    private readonly ILogger<PlaylistService> logger;
    private readonly IMusicPlayer player;

    public PlaylistService(MusicContext dbContext, Mp3Library mp3Library, ILogger<PlaylistService> logger, IMusicPlayer player)
    {
        this.dbContext = dbContext;
        this.mp3Library = mp3Library;
        this.logger = logger;
        this.player = player;
    }
    private Shared.Song? findSong(PlaylistItem item) => mp3Library.Songs.Where(s => s.Path == item.SongPath).FirstOrDefault();

    public async Task<IEnumerable<Shared.Playlist>> GetPlaylistsAsync()
    {
        var dbPlaylists = await dbContext.Playlists.Include(p => p.Songs).ToListAsync();
        logger.LogInformation("Found {count} playlists in database.", dbPlaylists.Count);
        return dbPlaylists.Select(p => new Shared.Playlist(p.Name, p.Songs.OrderBy(s => s.Order).Select(i => findSong(i))));
    }

    public async Task AppendSongToPlaylistAsync(string playlistName, string songPath)
    {
        logger.LogInformation("Adding {songPath} to {playlist} playlist", songPath, playlistName);

        var playlist = await dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == playlistName);
        if (playlist == null)
        {
            playlist = new Playlist
            {
                Name = playlistName
            };
            await dbContext.Playlists.AddAsync(playlist);
            await dbContext.SaveChangesAsync();
        }
        var playlistItem = new PlaylistItem
        {
            PlaylistId = playlist.Id,
            SongPath = songPath,
            Order = playlist.Songs.Count
        };
        await dbContext.PlaylistItems.AddAsync(playlistItem);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveSongFromPlaylistAsync(string playlistName, string songPath)
    {
        var playlist = await dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == playlistName);
        if (playlist == null)
        {
            logger.LogWarning("User tried to remove {song} from {playlistName} but that playlist doesn't exist.", songPath, playlistName);
            return;
        }
        var playlistItem = await dbContext.PlaylistItems.FirstOrDefaultAsync(i => i.PlaylistId == playlist.Id && i.SongPath == songPath);
        if (playlistItem == null)
        {
            logger.LogWarning("User tried to remove {song} from {playlistName} but that song isn't in that playlist.", songPath, playlistName);
            return;
        }

        logger.LogInformation("Removing {song} from {playlistName}", songPath, playlistName);
        dbContext.PlaylistItems.Remove(playlistItem);
        await dbContext.SaveChangesAsync();
    }

    public async Task PlayPlaylistAsync(string playlistName)
    {
        var playlist = await dbContext.Playlists.Include(p => p.Songs).FirstOrDefaultAsync(p => p.Name == playlistName);
        if (playlist == null)
        {
            logger.LogWarning("Asked to play playlist {playlistName} but it doesn't exist.", playlistName);
            return;
        }

        logger.LogInformation("Beginning to play playlist {playlistName}", playlistName);

        player.Stop();
        foreach (var playlistItem in playlist.Songs.OrderBy(s => s.Order))
        {
            var song = mp3Library.Songs.Single(s => s.Path == playlistItem.SongPath);
            player.EnqueueSong(song);
        }
    }
}