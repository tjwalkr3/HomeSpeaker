using HomeSpeaker.Shared;
using MvvmHelpers;

namespace HomeSpeaker.Maui.ViewModels;

public partial class StarredViewModel : BaseViewModel
{
    public StarredViewModel(IStaredSongDb database, HomeSpeakerClientProvider clientProvider)
    {
        Songs = new ObservableCollection<SongGroup>();
        this.database = database;
        this.clientProvider = clientProvider;
        Title = "Starred";
    }

    private readonly IStaredSongDb database;
    private readonly HomeSpeakerClientProvider clientProvider;
    [ObservableProperty]
    private string title;
    public ObservableCollection<SongGroup> Songs { get; private set; }
    public ObservableRangeCollection<SongViewModel> Songs2 { get; private set; } = new();
    public ObservableRangeCollection<Grouping<SongGroup, SongViewModel>> Songs2Groups { get; private set; } = new();

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
    public bool StatusIsVisible => String.IsNullOrWhiteSpace(Status) is false;

    [RelayCommand]
    private async Task ViewModelLoading()
    {
        Status = "getting song info...";
        Songs.Clear();
        var groups = new Dictionary<string, List<SongViewModel>>();
        var getSongsReply = clientProvider.Client.GetSongs(new GetSongsRequest { });
        var starredSongs = (await database.GetStarredSongsAsync()).Select(s => s.Path).ToList();
        await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
        {
            foreach (var s in reply.Songs.Where(s => starredSongs.Contains(s.Path)))
            {
                var song = s.ToSongViewModel();
                if (groups.ContainsKey(song.Folder) is false)
                    groups[song.Folder] = new List<SongViewModel>();
                groups[song.Folder].Add(song);

                Songs2.Add(song);
            }
        }

        foreach (var folder in Songs2.Select(s => s.Folder).Distinct())
        {
            var songsInFolder = Songs2.Where(s => s.Folder == folder).ToList();
            var key = new SongGroup(folder, songsInFolder);
            Songs2Groups.Add(new Grouping<SongGroup, SongViewModel>(key, songsInFolder));
        }

        foreach (var group in groups.OrderBy(g => g.Key))
        {
            Songs.Add(new SongGroup(group.Key, group.Value.OrderBy(s => s.Path).ToList()));
        }

        Status = null;

        // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
        //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        updateTitle();
    }

    private void updateTitle() => Title = $"Starred ({Songs.Count:n0} folders)";

    [RelayCommand]
    public void PlayFolder(SongGroup songs)
    {
        clientProvider.Client.PlayerControl(new PlayerControlRequest { Stop = true, ClearQueue = true });
        foreach (var s in songs)
        {
            clientProvider.Client.EnqueueSong(new PlaySongRequest { SongId = s.SongId });
        }
    }

    [RelayCommand]
    public void EnqueueFolder(SongGroup songs)
    {
        foreach (var s in songs)
        {
            clientProvider.Client.EnqueueSong(new PlaySongRequest { SongId = s.SongId });
        }
    }

    [RelayCommand]
    public async Task UnStarFolder(SongGroup songs)
    {
        foreach (var s in songs)
        {
            await database.DeleteStarredSong(new StarredSong { Path = s.Path });
        }
        Songs.Remove(songs);
        updateTitle();
    }
}