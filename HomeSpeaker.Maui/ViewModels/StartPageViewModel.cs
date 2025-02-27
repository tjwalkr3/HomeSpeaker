using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Models;
using HomeSpeaker.Maui.Services;
using System.Buffers.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
namespace HomeSpeaker.Maui.ViewModels;

public partial class StartPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _baseUrl = string.Empty;
    
    public ObservableCollection<string> Servers { get; } = [];

    private readonly IPlayerContext _context;
    public StartPageViewModel(IPlayerContext playerContext)
    {
        _context = playerContext;
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
}
