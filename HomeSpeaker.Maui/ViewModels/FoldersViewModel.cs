namespace HomeSpeaker.Maui.ViewModels;

public partial class FoldersViewModel : BaseViewModel
{
    public FoldersViewModel(IStaredSongDb staredSongDb, IPlayerService playerService)
    {
        Title = "Folders";
        Songs = new ObservableCollection<SongGroup>();

        this.staredSongDb = staredSongDb;
        this.playerService = playerService;
    }

    private readonly IStaredSongDb
        staredSongDb;
    private readonly IPlayerService playerService;

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


    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ResetFilterCommand))]
    private string filterText;

    [ObservableProperty]
    private IEnumerable<SongGroup> filteredSongs;

    [RelayCommand(CanExecute = nameof(CanResetFilter))]
    public void ResetFilter()
    {
        FilterText = string.Empty;
        FilteredSongs = Songs;
        Title = $"Folders ({Songs.Count:n0})";
    }

    public bool CanResetFilter() => !string.IsNullOrEmpty(FilterText);

    [RelayCommand]
    public void PerformFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            FilteredSongs = Songs;
            Title = $"Folders ({Songs.Count:n0})";
            return;
        }

        FilteredSongs = Songs.Where(s =>
            s.FolderName.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
            s.FolderPath.Contains(FilterText, StringComparison.OrdinalIgnoreCase)
        );
        Title = $"Filtered ({filteredSongs.Count():n0})";
    }

    [RelayCommand]
    public async Task Loading()
    {
        try
        {
            Status = "getting song info...";
            Songs.Clear();
            var groups = await playerService.GetSongGroups();
            foreach (var group in groups.OrderBy(g => g.Key))
            {
                Songs.Add(new SongGroup(group.Key, group.Value.OrderBy(s => s.Path).ToList()));
            }

            Status = null;
            FilteredSongs = Songs;
            Title = $"Folders ({Songs.Count:n0})";

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
        playerService.PlayFolder(songs);
    }

    [RelayCommand]
    public void EnqueueFolder(SongGroup songs)
    {
        playerService.EnqueueFolder(songs);
    }

    [RelayCommand]
    public async Task StarFolder(SongGroup songs)
    {
        foreach (var s in songs)
        {
            await staredSongDb.SaveStarredSongAsync(new StarredSong { Path = s.Path });
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
