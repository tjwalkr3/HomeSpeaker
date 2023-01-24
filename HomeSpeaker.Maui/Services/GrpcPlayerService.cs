using HomeSpeaker.Shared;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Maui.Services;

public interface IPlayerService
{
    Task PlayFolderAsync(SongGroup songs);
    Task EnqueueFolderAsync(SongGroup songs);
    Task PlaySongAsync(int songId);
    Task EnqueueSongAsync(int songId);
    Task<Dictionary<string, List<SongViewModel>>> GetSongGroups();
    Task<IEnumerable<string>> GetFolders();
    Task<IEnumerable<SongViewModel>> GetSongsInFolder(string folder);
}

public class GrpcPlayerService : IPlayerService
{
    private readonly HomeSpeakerClientProvider clientProvider;
    private readonly IStaredSongDb database;
    private readonly ILogger<GrpcPlayerService> logger;

    public GrpcPlayerService(HomeSpeakerClientProvider clientProvider, IStaredSongDb database, ILogger<GrpcPlayerService> logger)
    {
        this.clientProvider = clientProvider;
        this.database = database;
        this.logger = logger;
    }

    public async Task EnqueueFolderAsync(SongGroup songs)
    {
        foreach (var s in songs)
        {
            await clientProvider.Client.EnqueueSongAsync(new PlaySongRequest { SongId = s.SongId });
        }
    }

    public async Task PlayFolderAsync(SongGroup songs)
    {
        await clientProvider.Client.PlayerControlAsync(new PlayerControlRequest { Stop = true, ClearQueue = true });
        foreach (var s in songs)
        {
            await clientProvider.Client.EnqueueSongAsync(new PlaySongRequest { SongId = s.SongId });
        }
    }

    public async Task<IEnumerable<SongViewModel>> GetSongsInFolder(string folder)
    {
        var songs = new List<SongViewModel>();
        var getSongsReply = clientProvider.Client.GetSongs(new GetSongsRequest { Folder = folder });
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs)
            {
                songs.Add(s.ToSongViewModel());
            }
        }
        return songs;
    }

    readonly char[] separators = new[] { '/', '\\' };

    public async Task<IEnumerable<string>> GetFolders()
    {
        List<string> folders = new();

        logger.LogInformation("User wanted folders, so first I'll get all the songs.");

        var getSongsReply = clientProvider.Client.GetSongs(new GetSongsRequest { });
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs)
            {
                var parts = s.Path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                //var directory = s.Path.Replace(parts.Last(), string.Empty);
                var directory = parts[0];

                if (!folders.Contains(directory))
                {
                    logger.LogInformation("Found directory {directory} from path {path}", directory, s.Path);
                    folders.Add(directory);
                }
            }
        }

        return folders;
    }

    public async Task<Dictionary<string, List<SongViewModel>>> GetSongGroups()
    {
        var groups = new Dictionary<string, List<SongViewModel>>();
        var getSongsReply = clientProvider.Client.GetSongs(new GetSongsRequest { });
        var starredSongs = (await database.GetStarredSongsAsync()).Select(s => s.Path).ToList();
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs.Where(s => starredSongs.Contains(s.Path) == false))
            {
                var song = s.ToSongViewModel();
                if (groups.ContainsKey(song.Folder) is false)
                    groups[song.Folder] = new List<SongViewModel>();
                groups[song.Folder].Add(song);
            }
        }

        return groups;
    }

    public async Task PlaySongAsync(int songId)
    {
        await clientProvider.Client.PlayerControlAsync(new PlayerControlRequest { Stop = true, ClearQueue = true });
        await clientProvider.Client.EnqueueSongAsync(new PlaySongRequest { SongId = songId });
    }

    public async Task EnqueueSongAsync(int songId)
    {
        await clientProvider.Client.EnqueueSongAsync(new PlaySongRequest { SongId = songId });
    }
}
