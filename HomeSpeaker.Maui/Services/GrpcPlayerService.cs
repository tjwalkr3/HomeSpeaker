using HomeSpeaker.Shared;

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

    public GrpcPlayerService(HomeSpeakerClientProvider clientProvider, IStaredSongDb database)
    {
        this.clientProvider = clientProvider;
        this.database = database;
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

    public async Task<IEnumerable<string>> GetFolders()
    {
        List<string> folders = new();

        var getSongsReply = clientProvider.Client.GetSongs(new GetSongsRequest { });
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs)
            {
                var directory = Path.GetDirectoryName(s.Path);
                if (!folders.Contains(directory))
                    folders.Add(directory);
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
