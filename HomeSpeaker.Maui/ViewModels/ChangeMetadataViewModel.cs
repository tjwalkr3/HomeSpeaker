using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Models;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Shared;
using System.Collections.ObjectModel;

namespace HomeSpeaker.Maui.ViewModels;

public partial class ChangeMetadataViewModel(IMauiHomeSpeakerService hsService) : ObservableObject
{

    [ObservableProperty]
    SongModel? selectedSong;

    [ObservableProperty]
    ObservableCollection<SongModel> allSongsList = new();
    [ObservableProperty]
    string songName;
    [ObservableProperty]
    string artist;
    [ObservableProperty]
    string album;

    [RelayCommand]
    public async Task GetAllSongs()
    {
        List<SongModel> newSongsList = (await hsService.GetAllSongsAsync()).ToList();
        AllSongsList.Clear();
        foreach (var song in newSongsList)
        {
            AllSongsList.Add(song);
        }
    }
    [RelayCommand]
    public async Task UpdateMetadata()
    {
        if (SelectedSong == null)
            return;

        var song = new SongMessage
        {
            SongId = SelectedSong.SongId,
            Name = SongName,
            Artist = Artist,
            Album = Album
        };

        await hsService.UpdateMetadataAsync(song);

        ResetValues();
    }

    private void ResetValues()
    {
        SongName = string.Empty;
        Artist = string.Empty;
        Album = string.Empty;
    }
}
