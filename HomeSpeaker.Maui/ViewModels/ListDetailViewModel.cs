namespace HomeSpeaker.Maui.ViewModels;

public partial class ListDetailViewModel : BaseViewModel
{
    readonly SampleDataService dataService;
    private readonly IPlayerService playerService;
    public ListDetailViewModel(SampleDataService service, IPlayerService playerService)
    {
        dataService = service;
        this.playerService = playerService;
    }

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsStatusVisible))]
    private string status;

    [ObservableProperty]
    private string title;

    public ObservableCollection<string> Folders { get; } = new();

    public bool IsStatusVisible => !string.IsNullOrEmpty(status);

    [RelayCommand]
    public async Task Loading()
    {
        try
        {
            Status = "getting song info...";
            Folders.Clear();
            foreach (var folder in (await playerService.GetFolders()).Order())
                Folders.Add(folder);

            Status = null;
            Title = $"Folders ({Folders.Count:n0})";
        }
        catch (Exception ex)
        {
            Status = ex.ToString();
        }
    }

    [RelayCommand]
    private async void GoToDetails(string folder)
    {
        await Shell.Current.GoToAsync(nameof(ListDetailDetailPage), true, new Dictionary<string, object>
        {
            { "Folder", folder }
        });
    }
}
