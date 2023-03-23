namespace HomeSpeaker.WebAssembly.Services;

public class HomeSpeakerService
{
    private HomeSpeaker.Shared.HomeSpeaker.HomeSpeakerClient client;
    private List<SongMessage> songs = new();
    public IEnumerable<SongMessage> Songs => songs;

    public HomeSpeakerService(IConfiguration config, ILogger<HomeSpeakerService> logger)
    {
        var channel = GrpcChannel.ForAddress(config["ServerAddress"] ?? throw new MissingConfigException("ServerAddress"), new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        });

        client = new HomeSpeaker.Shared.HomeSpeaker.HomeSpeakerClient(channel);
        this.logger = logger;

        _ = listenForEvents();
    }

    private async Task listenForEvents()
    {
        var eventReply = client.SendEvent(new Google.Protobuf.WellKnownTypes.Empty());
        await foreach (var eventInstance in eventReply.ResponseStream.ReadAllAsync())
        {
            StatusChanged?.Invoke(this, eventInstance.Message);
        }
    }

    public async Task<GetStatusReply> GetStatusAsync()
    {
        return await client.GetPlayerStatusAsync(new GetStatusRequest());
    }

    public async Task EnqueueFolderAsync(SongGroup songs)
    {
        foreach (var s in songs)
        {
            await client.EnqueueSongAsync(new PlaySongRequest { SongId = s.SongId });
        }
    }

    public async Task PlayFolderAsync(SongGroup songs)
    {
        await client.PlayerControlAsync(new PlayerControlRequest { Stop = true, ClearQueue = true });
        foreach (var s in songs)
        {
            await client.EnqueueSongAsync(new PlaySongRequest { SongId = s.SongId });
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

    readonly char[] separators = new[] { '/', '\\' };
    private readonly ILogger<HomeSpeakerService> logger;

    public async Task<IEnumerable<string>> GetFolders()
    {
        List<string> folders = new();

        logger.LogInformation("User wanted folders, so first I'll get all the songs.");

        var getSongsReply = client.GetSongs(new GetSongsRequest { });
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
        var getSongsReply = client.GetSongs(new GetSongsRequest { });
        //var starredSongs = (await database.GetStarredSongsAsync()).Select(s => s.Path).ToList();
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs/*.Where(s => starredSongs.Contains(s.Path) == false)*/)
            {
                var song = s.ToSongViewModel();
                if (song.Folder == null)
                    continue;

                if (groups.ContainsKey(song.Folder) is false)
                    groups[song.Folder] = new List<SongViewModel>();
                groups[song.Folder].Add(song);
            }
        }

        return groups;
    }

    public async Task PlaySongAsync(int songId)
    {
        await client.PlayerControlAsync(new PlayerControlRequest { Stop = true, ClearQueue = true });
        await client.EnqueueSongAsync(new PlaySongRequest { SongId = songId });
    }

    public async Task EnqueueSongAsync(int songId)
    {
        await client.EnqueueSongAsync(new PlaySongRequest { SongId = songId });
    }

    public async Task PlayFolderAsync(string folder)
    {
        await client.PlayFolderAsync(new PlayFolderRequest { FolderPath = folder });
    }

    public async Task EnqueueFolderAsync(string folder)
    {
        await client.EnqueueFolderAsync(new EnqueueFolderRequest { FolderPath = folder });
    }

    public async Task StopPlayingAsync() => await client.PlayerControlAsync(new PlayerControlRequest { Stop = true });
    public async Task ClearQueueAsync() => await client.PlayerControlAsync(new PlayerControlRequest { ClearQueue = true });
    public async Task SkipToNextAsync() => await client.PlayerControlAsync(new PlayerControlRequest { SkipToNext = true });
    public async Task ResumePlayAsync() => await client.PlayerControlAsync(new PlayerControlRequest { Play = true });

    public event EventHandler<string>? StatusChanged;
}
