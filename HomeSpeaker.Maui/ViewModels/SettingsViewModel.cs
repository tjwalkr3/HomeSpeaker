using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Maui.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly HomeSpeakerClientProvider clientProvider;
    private readonly ILogger<SettingsViewModel> logger;

    public ObservableCollection<string> PastServers { get; } = new();

    public SettingsViewModel(HomeSpeakerClientProvider clientProvider, ILogger<SettingsViewModel> logger)
    {
        this.clientProvider = clientProvider;
        this.logger = logger;
        deserializePastServers();
        CurrentServerAddress = Preferences.Get(Constants.ServerAddress, "Unknown");
    }

    public string Title => "Settings";

    [ObservableProperty]
    private string currentServerAddress;

    [ObservableProperty]
    private string newServerAddress;

    [RelayCommand]
    private void DeleteServer(string serverToDelete)
    {
        PastServers.Remove(serverToDelete);
        serializePastServers();
    }

    [RelayCommand]
    private void ConnectServer(string serverAddress)
    {
        Preferences.Set(Constants.ServerAddress, serverAddress);
        clientProvider.ReloadClientFromPreferences();
        CurrentServerAddress = serverAddress;
    }

    [ObservableProperty]
    private string errorMessage;

    [RelayCommand]
    public void AddServer()
    {
        PastServers.Add(NewServerAddress);
        serializePastServers();
        NewServerAddress = null;
    }

    private void deserializePastServers()
    {
        var pastServersJson = Preferences.Get(Constants.PastServers, "");
        if (pastServersJson != string.Empty)
        {
            var pastServers = JsonSerializer.Deserialize<IEnumerable<string>>(pastServersJson);
            foreach (var pastServer in pastServers)
            {
                PastServers.Add(pastServer);
            }
        }
    }

    private void serializePastServers()
    {
        var pastServersJson = JsonSerializer.Serialize<IEnumerable<string>>(PastServers);
        Preferences.Set(Constants.PastServers, pastServersJson);
    }
}
