using Grpc.Core;
using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public class FoldersViewModel : BaseViewModel
{
    public FoldersViewModel(Database database, HomeSpeakerClient client)
    {
        Title = "Folders";
        Songs = new ObservableCollection<SongGroup>();
        this.client = client;
        init();
        this.database = database;
    }

    private readonly HomeSpeakerClient client;
    private readonly Database database;

    public Command LoginCommand { get; }
    public string Title { get; }
    public ObservableCollection<SongGroup> Songs { get; private set; }

    private string status;
    public string Status
    {
        get => status;
        set
        {
            SetProperty(ref status, value);
            OnPropertyChanged(nameof(StatusIsVisible));
        }
    }
    public bool StatusIsVisible => string.IsNullOrWhiteSpace(Status) is false;

    private async void init()
    {
        Status = "getting song info...";
        var groups = new Dictionary<string, List<SongViewModel>>();
        var getSongsReply = client.GetSongs(new GetSongsRequest { });
        var starredSongs = (await database.GetStarredSongsAsync()).Select(s => s.Path).ToList();
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs.Where(s => starredSongs.Contains(s.Path) == false))
            {
                var song = s.ToSongViewModel(database, client);
                if (groups.ContainsKey(song.Folder) is false)
                    groups[song.Folder] = new List<SongViewModel>();
                groups[song.Folder].Add(song);
            }
        }

        foreach (var group in groups.OrderBy(g => g.Key))
        {
            Songs.Add(new SongGroup(group.Key, group.Value.OrderBy(s => s.Path).ToList(), database));
        }

        Status = null;

        // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
        //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");

    }
}

public static class ViewModelExtensions
{
    public static SongViewModel ToSongViewModel(this SongMessage song, Database database, HomeSpeakerClient client)
    {
        return new SongViewModel(database, client)
        {
            SongId = song?.SongId ?? -1,
            Name = song?.Name ?? "[ Null Song Response ??? ]",
            Album = song?.Album,
            Artist = song?.Artist,
            Path = song?.Path
        };
    }
    public async static IAsyncEnumerable<T> ReadAllAsync<T>(this IAsyncStreamReader<T> streamReader, CancellationToken cancellationToken = default)
    {
        if (streamReader == null)
        {
            throw new System.ArgumentNullException(nameof(streamReader));
        }

        while (await streamReader.MoveNext(cancellationToken))
        {
            yield return streamReader.Current;
        }
    }
}