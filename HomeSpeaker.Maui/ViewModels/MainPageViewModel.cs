using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Services;
using HomeSpeaker.Maui.Models;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class MainPageViewModel(IMauiHomeSpeakerService hsService) : ObservableObject
{
    public ObservableCollection<SongModel> AllSongsList { get; set; } = [];

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
}
