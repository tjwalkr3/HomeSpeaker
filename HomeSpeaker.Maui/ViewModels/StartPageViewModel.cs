using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class StartPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _baseUrl = "https://localhost:7238";

    public ObservableCollection<string> Servers { get; } = [];

    private readonly IPlayerContext _context;
    public StartPageViewModel(IPlayerContext playerContext)
    {
        _context = playerContext;
        LoadServers();
    }

    private void LoadServers()
    {
        Servers.Clear();
        foreach (var server in _context.Services)
        {
            Servers.Add(server.ServerAddress);
        }
    }

    public bool NewServerValid()
    {
        Uri? uriResult;
        if (Uri.TryCreate(BaseUrl, UriKind.Absolute, out uriResult) && uriResult != null &&
           (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [RelayCommand(CanExecute = nameof(NewServerValid))]
    public async Task AddNewServer()
    {
        await _context.AddService(BaseUrl);
        Servers.Add(BaseUrl);
        StartControllingCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public async Task StartControlling(string baseURL)
    {
        await _context.SetCurrentService(baseURL);
        await Shell.Current.GoToAsync("///MainPage");
    }

    private void ResetValue()
    {
        BaseUrl = string.Empty;
    }
}
