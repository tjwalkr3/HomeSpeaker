using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Maui.ViewModels;

public partial class ListDetailViewModel : BaseViewModel
{
    readonly SampleDataService dataService;
    private readonly IPlayerService playerService;
    private readonly ILogger<ListDetailViewModel> logger;

    public ListDetailViewModel(SampleDataService service, IPlayerService playerService, ILogger<ListDetailViewModel> logger)
    {
        dataService = service;
        this.playerService = playerService;
        this.logger = logger;

        logger.LogInformation("Creating a ListDetailViewModel");
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
            logger.LogWarning("Loading folder names");

            Status = "getting song info...";
            Folders.Clear();
            foreach (var folder in (await playerService.GetFolders()).Order())
            {
                Folders.Add(folder);
                logger.LogInformation("Found {folder}", folder);
            }

            Status = null;
            Title = $"Folders ({Folders.Count:n0})";

            logger.LogError("Found {folderCount} folders", Folders.Count);
        }
        catch (Exception ex)
        {
            Status = ex.ToString();
            logger.LogError(ex, "Trouble loading folders");
        }
    }

    [RelayCommand]
    private async void GoToDetails(string folder)
    {
        await Shell.Current.GoToAsync(nameof(ListDetailDetailPage), true, new Dictionary<string, object>
        {
            //Use either syntax.  Pick one.  This lets you pass full objects as navagition parameters.
            //{ "Folder", folder },
            ["Folder"] = folder
        });
    }

    [RelayCommand]
    private async Task PlayFolder(string folder)
    {
        await playerService.PlayFolderAsync(folder);
    }

    [RelayCommand]
    private async Task EnqueueFolder(string folder)
    {
        await playerService.EnqueueFolderAsync(folder);
    }
}
