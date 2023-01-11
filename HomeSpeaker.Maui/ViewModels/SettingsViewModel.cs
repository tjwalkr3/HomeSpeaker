using Grpc.Net.Client;
using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly HomeSpeakerClient client;

    public SettingsViewModel(HomeSpeakerClient client)
    {
        this.client = client;
        ServerAddress = Preferences.Get(Constants.ServerAddress, "Unknown");
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
        var baseUri = new Uri(serverAddress);
        var channel = GrpcChannel.ForAddress(baseUri);
        var newClient = new HomeSpeakerClient(channel);

        Preferences.Set(Constants.ServerAddress, serverAddress);
    }
}
