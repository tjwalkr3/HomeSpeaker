using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.Services;

public interface IPlayerService
{
    void PlayFolder(SongGroup songs);
    void EnqueueFolder(SongGroup songs);
    void PlaySong(int songId);
    void EnqueueSong(int songId);
    Task<Dictionary<string, List<SongViewModel>>> GetSongGroups();
    Task<IEnumerable<string>> GetFolders();
    Task<IEnumerable<SongViewModel>> GetSongsInFolder(string folder);
}

public class GrpcPlayerService : IPlayerService
{
    private readonly HomeSpeakerClient client;
    private readonly IStaredSongDb database;

    public GrpcPlayerService(HomeSpeakerClient grpcClient, IStaredSongDb database)
    {
        this.client = grpcClient;
        this.database = database;
    }

    public void EnqueueFolder(SongGroup songs)
    {
        foreach (var s in songs)
        {
            client.EnqueueSong(new PlaySongRequest { SongId = s.SongId });
        }
    }

    public void PlayFolder(SongGroup songs)
    {
        client.PlayerControl(new PlayerControlRequest { Stop = true, ClearQueue = true });
        foreach (var s in songs)
        {
            client.EnqueueSong(new PlaySongRequest { SongId = s.SongId });
        }
    }

    public async Task<IEnumerable<SongViewModel>> GetSongsInFolder(string folder)
    {
        var songs = new List<SongViewModel>();
        var getSongsReply = client.GetSongs(new GetSongsRequest { Folder = folder });
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

        var getSongsReply = client.GetSongs(new GetSongsRequest { });
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
        var getSongsReply = client.GetSongs(new GetSongsRequest { });
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

    public void PlaySong(int songId)
    {
        client.PlayerControl(new PlayerControlRequest { Stop = true, ClearQueue = true });
        client.EnqueueSong(new PlaySongRequest { SongId = songId });
    }

    public void EnqueueSong(int songId)
    {
        client.EnqueueSong(new PlaySongRequest { SongId = songId });
    }
}
