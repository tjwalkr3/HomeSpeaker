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
    }

    [RelayCommand]
    public async Task GetAllSongs()
    {
        AllSongsList.Clear();
        if (_context.CurrentService == null) return;
        List<SongModel> newSongsList = (await _context.CurrentService.GetAllSongsAsync()).ToList();
        foreach (var song in newSongsList)
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
}
