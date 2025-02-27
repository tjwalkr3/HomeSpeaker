using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Maui.Models;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    public ObservableCollection<SongModel> AllSongsList { get; } = [];

    private readonly IPlayerContext _context;
    public MainPageViewModel(IPlayerContext playerContext)
    {
        _context = playerContext;
        LoadSongs();
    }

    private void LoadSongs()
    {
        AllSongsList.Clear();
        foreach (var song in _context.Songs)
        {
            AllSongsList.Add(song);
        }
    }

    [RelayCommand]
    public async Task PlaySong(int songId)
    {
        if (_context.CurrentService == null) return;
        await _context.CurrentService.PlaySongAsync(songId);
    }

    [RelayCommand]
    public async Task NavigateToStart()
    {
        await Shell.Current.GoToAsync("///StartPage");
    }

    [RelayCommand]
    public async Task NavigateToEditor()
    {
        await Shell.Current.GoToAsync("///ChangeMetadata");
    }

    [RelayCommand]
    public async Task NavigateToPlaylist()
    {
        await Shell.Current.GoToAsync("///PlaylistPage");
    }
}
