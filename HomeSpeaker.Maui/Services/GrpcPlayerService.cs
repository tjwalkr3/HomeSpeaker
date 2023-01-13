using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.Services;

public interface IPlayerService
{
    void PlayFolder(SongGroup songs);
    void EnqueueFolder(SongGroup songs);
    Task<Dictionary<string, List<SongViewModel>>> GetSongGroups();
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
}
