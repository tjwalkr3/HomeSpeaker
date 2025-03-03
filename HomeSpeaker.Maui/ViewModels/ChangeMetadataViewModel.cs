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
    private readonly INavigationService _navigationService;

    public ChangeMetadataViewModel(IPlayerContext playerContext, INavigationService navigationService)
    {
        _context = playerContext ?? throw new ArgumentNullException(nameof(playerContext));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        LoadSongs();
    }

    public void LoadSongs()
    {
        AllSongsList.Clear();
        foreach (var song in _context.Songs)
        {
            AllSongsList.Add(song);
        }
    }

    [RelayCommand]
    private async Task UpdateMetadata()
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

    [RelayCommand]
    private async Task NavigateToMainPage()
    {
        await _navigationService.GoToAsync("///MainPage");
    }
}
