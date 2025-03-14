using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
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

    //[RelayCommand]
    //private void AddSongToPlaylist(Playlist playlist)
    //{
    //    if (playlist == null) return;

    //    var newSong = new Song { Name = "New Song", Artist = "Unknown", Album = "Unknown", Path = "Path/to/song.mp3" };
    //    playlist.Songs.Add(newSong);
    //}

    [RelayCommand]
    private async Task RemoveFromPlaylist(Playlist playlist)
    {
        if (_context.CurrentService == null || playlist == null) return;

        var songToRemove = playlist.Songs.FirstOrDefault();
        if (songToRemove == null) return;

        await _context.CurrentService.RemoveFromPlaylistAsync(playlist.Name, songToRemove.Path);
        playlist.Songs.Remove(songToRemove);
    }


    [RelayCommand]
    private async Task NavigateToMainPage()
    {
        await _navigationService.GoToAsync("///MainPage");
    }
}
