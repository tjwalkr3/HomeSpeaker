using HomeSpeaker.Server2.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeSpeaker.Server;

public class PlaylistService
{
    private readonly MusicContext dbContext;
    private readonly Mp3Library mp3Library;

    public PlaylistService(MusicContext dbContext, Mp3Library mp3Library)
    {
        this.dbContext = dbContext;
        this.mp3Library = mp3Library;
    }
    private Shared.Song? findSong(PlaylistItem item) => mp3Library.Songs.Where(s => s.Path == item.SongPath).FirstOrDefault();

    public async Task<IEnumerable<Shared.Playlist>> GetPlaylistsAsync()
    {
        var dbPlaylists = await dbContext.Playlists.Include(p => p.Songs).ToListAsync();
        return dbPlaylists.Select(p => new Shared.Playlist(p.Name, p.Description, p.Songs.OrderBy(s => s.Order).Select(i => findSong(i))));
    }

    public async Task AppendSongToPlaylistAsync(string playlistName, string songPath)
    {
        var playlist = await dbContext.Playlists.FirstOrDefaultAsync(p => p.Name == playlistName);
        if (playlist == null)
        {
            playlist = new Server2.Data.Playlist
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
}