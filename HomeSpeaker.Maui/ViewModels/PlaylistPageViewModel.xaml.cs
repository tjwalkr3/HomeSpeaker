using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;

namespace HomeSpeaker.Maui.ViewModels;

public partial class PlaylistPageViewModel : ObservableObject
{
    public ObservableCollection<Playlist> Playlists { get; private set; } = new();

    private readonly IPlayerContext _context;
    private readonly INavigationService _navigationService;

    public PlaylistPageViewModel(IPlayerContext playerContext, INavigationService navigationService)
    {
        _context = playerContext ?? throw new ArgumentNullException(nameof(playerContext));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    [RelayCommand]
    private async Task GetAllPlaylists()
    {
        Playlists.Clear();
        if (_context.CurrentService == null) return;

        var playlists = await _context.CurrentService!.GetPlaylistsAsync();
        foreach (var playlist in playlists)
        {
            Playlists.Add(playlist);
        }
    }

    [RelayCommand]
    private async Task PlayPlaylist(Playlist playlist)
    {
        if (_context.CurrentService == null) return;
        await _context.CurrentService!.PlayPlaylistAsync(playlist.Name);
    }

    [RelayCommand]
    private void ToggleExpand(Playlist playlist)
    {
        if (playlist != null && _context.CurrentService != null)
        {
            playlist.IsExpanded = !playlist.IsExpanded;
        }
    }

    [RelayCommand]
    private async Task NavigateToMainPage()
    {
        await _navigationService.GoToAsync("///MainPage");
    }
}
