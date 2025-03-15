using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using Mopups.Services;

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
            playlist.IsExpanded = true;
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
        if (playlist.IsExpanded2)
        {
            playlist.IsExpanded2 = false;

            foreach (var p in Playlists)
            {
                p.IsExpanded = true;
            }
        }
        else
        {
            foreach (var p in Playlists)
            {
                if (p != playlist)
                {
                    p.IsExpanded = false;
                }
            }

            playlist.IsExpanded2 = true;
        }
    }

    [RelayCommand]
    private async Task RemoveFromPlaylist(Song song)
    {
        if (_context.CurrentService == null || song == null) return;

        var playlist = Playlists.FirstOrDefault(p => p.Songs.Contains(song));
        if (playlist == null) return;

        await _context.CurrentService.RemoveFromPlaylistAsync(playlist.Name, song.Path);
        playlist.Songs.Remove(song);
    }

    [RelayCommand]
    private async Task NavigateToMainPage()
    {
        await _navigationService.GoToAsync("///MainPage");
    }
}
