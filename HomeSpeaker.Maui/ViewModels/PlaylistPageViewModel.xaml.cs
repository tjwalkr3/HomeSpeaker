using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;

namespace HomeSpeaker.Maui.ViewModels;

public partial class PlaylistPageViewModel(IMauiHomeSpeakerService hsService) : ObservableObject
{

    public ObservableCollection<Playlist> Playlists { get; private set; } = new();

    [RelayCommand]
    public async Task GetAllPlaylists()
    {
        var playlists = await hsService.GetPlaylistsAsync();
        Playlists.Clear();
        foreach (var playlist in playlists)
        {
            Playlists.Add(playlist);
        }
    }

    [RelayCommand]
    public async Task PlayPlaylist(Playlist playlist)
    {
        await hsService.PlayPlaylistAsync(playlist.Name);
    }

    [RelayCommand]
    private void ToggleExpand(Playlist playlist)
    {
        if (playlist != null)
        {
            playlist.IsExpanded = !playlist.IsExpanded;
        }
    }
}
