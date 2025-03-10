using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Maui.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace HomeSpeaker.Maui.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    public ObservableCollection<SongModel> AllSongsList { get; } = new();
    public ObservableCollection<SongModel> FilteredSongsList { get; } = new();

    private readonly IPlayerContext _context;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    public MainPageViewModel(IPlayerContext playerContext, INavigationService navigationService)
    {
        _context = playerContext ?? throw new ArgumentNullException(nameof(playerContext));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        LoadSongs();
        FilterSongs();
    }

    public void LoadSongs()
    {
        AllSongsList.Clear();
        foreach (var song in _context.Songs)
        {
            AllSongsList.Add(song);
        }
        FilterSongs();
    }

    partial void OnSearchQueryChanged(string value)
    {
        FilterSongs();
    }

    private void FilterSongs()
    {
        FilteredSongsList.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchQuery)
            ? AllSongsList.ToList()
            : AllSongsList
                .Where(song => song.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

        foreach (var song in filtered)
        {
            FilteredSongsList.Add(song);
        }
    }

    [RelayCommand]
    private async Task PlaySong(int songId)
    {
        if (_context.CurrentService == null) return;
        await _context.CurrentService.PlaySongAsync(songId);
    }

    [RelayCommand]
    private async Task NavigateToStart()
    {
        await _navigationService.GoToAsync("///StartPage");
    }

    [RelayCommand]
    private async Task NavigateToEditor()
    {
        await _navigationService.GoToAsync("///ChangeMetadata");
    }

    [RelayCommand]
    private async Task NavigateToPlaylist()
    {
        await _navigationService.GoToAsync("///PlaylistPage");
    }
}
