using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public partial class FoldersViewModel : BaseViewModel
{
    public FoldersViewModel(Database database, HomeSpeakerClient client)
    {
        Title = "Folders";
        Songs = new ObservableCollection<SongGroup>();
        this.client = client;
        this.database = database;
    }

    private readonly HomeSpeakerClient client;
    private readonly Database database;

    public Command LoginCommand { get; }
    [ObservableProperty]
    private string title;
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

    [RelayCommand]
    public async Task Loading()
    {
        try
        {
            Status = "getting song info...";
            Songs.Clear();
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
                Songs.Add(new SongGroup(group.Key, group.Value.OrderBy(s => s.Path).ToList()));
            }

            Status = null;

            Title = $"Folders ({Songs.Count:n0} songs)";

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
        catch (Exception ex)
        {
            Status = ex.ToString();
        }
    }

    [RelayCommand]
    public void PlayFolder(SongGroup songs)
    {
        client.PlayerControl(new PlayerControlRequest { Stop = true, ClearQueue = true });
        foreach (var s in songs)
        {
            client.EnqueueSong(new PlaySongRequest { SongId = s.SongId });
        }
    }

    [RelayCommand]
    public void EnqueueFolder(SongGroup songs)
    {
        foreach (var s in songs)
        {
            client.EnqueueSong(new PlaySongRequest { SongId = s.SongId });
        }
    }

    [RelayCommand]
    public async Task StarFolder(SongGroup songs)
    {
        foreach (var s in songs)
        {
            await database.SaveStarredSongAsync(new StarredSong { Path = s.Path });
        }
        Songs.Remove(songs);
    }

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    private async Task OnRefreshing()
    {
        IsRefreshing = true;

        try
        {
            await Loading();
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
