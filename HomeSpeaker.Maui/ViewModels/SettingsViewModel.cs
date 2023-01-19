using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly HomeSpeakerClient client;
    private readonly GrpcClientProvider clientProvider;
    public ObservableCollection<string> PastServers { get; } = new();

    public SettingsViewModel(HomeSpeakerClient client, GrpcClientProvider clientProvider)
    {
        this.client = client;
        this.clientProvider = clientProvider;
        ServerAddress = Preferences.Get(Constants.ServerAddress, "Unknown");
        deserializePastServers();
    }

    public string Title => "Settings";

    [ObservableProperty]
    private string serverAddress;

    [RelayCommand]
    public async Task ViewModelLoading()
    {

    }

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private ObservableCollection<Song> songs = new();

    [RelayCommand]
    public void UpdateServerAddress()
    {
        Preferences.Set(Constants.ServerAddress, ServerAddress);
        clientProvider.ReloadClientFromPreferences();
        PastServers.Add(ServerAddress);
        serializePastServers();
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
