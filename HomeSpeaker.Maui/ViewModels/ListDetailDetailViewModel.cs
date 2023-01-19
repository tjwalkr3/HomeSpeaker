namespace HomeSpeaker.Maui.ViewModels;

[QueryProperty("Folder", "Folder")]
public partial class ListDetailDetailViewModel : BaseViewModel
{
    private readonly IPlayerService playerService;

    public ListDetailDetailViewModel(IPlayerService playerService)
    {
        this.playerService = playerService;
    }

    private string folder;
    public string Folder
    {
        get => folder;
        set
        {
            SetProperty(ref folder, value);
            RefreshSongsCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task RefreshSongs()
    {
        var songs = await playerService.GetSongsInFolder(folder);
        Songs.Clear();
        foreach (var s in songs.OrderBy(s => s.Name))
        {
            Songs.Add(s);
        }
    }

    public ObservableCollection<SongViewModel> Songs { get; } = new();

    [RelayCommand]
    private void PlaySong(SongViewModel song)
    {
        playerService.PlaySong(song.SongId);
    }

    [RelayCommand]
    private void EnqueueSong(SongViewModel song)
    {
        playerService.EnqueueSong(song.SongId);
    }
}
