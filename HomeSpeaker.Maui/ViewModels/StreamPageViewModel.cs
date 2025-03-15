using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Models;
using HomeSpeaker.Maui.Services;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class StreamPageViewModel : ObservableObject
{
    public ObservableCollection<StreamModel> AllStreamsList { get; } = [];

    private readonly IPlayerContext _context;
    private readonly INavigationService _navigationService;
    private readonly IMusicStreamService _musicStreamService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public StreamPageViewModel(IPlayerContext playerContext, INavigationService navigationService, IMusicStreamService musicStreamService)
    {
        _context = playerContext;
        _navigationService = navigationService;
        _musicStreamService = musicStreamService;
    }

    [RelayCommand]
    private async Task SearchStreams()
    {
        AllStreamsList.Clear();

        List<StreamModel> searchResult = string.IsNullOrWhiteSpace(SearchQuery)
            ? []
            : await _musicStreamService.Search(SearchQuery);

        foreach (var stream in searchResult)
        {
            AllStreamsList.Add(stream);
        }
    }

    [RelayCommand]
    private async Task PlayStream(string streamUrl)
    {
        if (_context.CurrentService == null) return;
        await _context.CurrentService.PlayStreamAsync(streamUrl);
    }

    private bool SearchQueryValid()
    {
        return !string.IsNullOrWhiteSpace(SearchQuery);
    }

    [RelayCommand]
    private async Task NavigateToMainPage()
    {
        await _navigationService.GoToAsync("///MainPage");
    }
}
