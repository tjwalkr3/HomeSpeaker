using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Models;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using System.Collections.ObjectModel;

namespace HomeSpeaker.Maui.ViewModels;

public partial class ChangeMetadataViewModel : ObservableObject
{
    [ObservableProperty]
    SongModel? selectedSong;

    [ObservableProperty]
    ObservableCollection<SongModel> allSongsList = new();

    [ObservableProperty]
    string songName = string.Empty;

    [ObservableProperty]
    string artist = string.Empty;

    [ObservableProperty]
    string album = string.Empty;

    private readonly IPlayerContext _context;
    public ChangeMetadataViewModel(IPlayerContext playerContext)
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
    public async Task UpdateMetadata()
    {
        if (SelectedSong == null || _context.CurrentService == null)
            return;

        var song = new SongMessage
        {
            SongId = SelectedSong.SongId,
            Name = SongName,
            Artist = Artist,
            Album = Album
        };

        await _context.CurrentService.UpdateMetadataAsync(song);

        ResetValues();
    }

    private void ResetValues()
    {
        SongName = string.Empty;
        Artist = string.Empty;
        Album = string.Empty;
    }
}
