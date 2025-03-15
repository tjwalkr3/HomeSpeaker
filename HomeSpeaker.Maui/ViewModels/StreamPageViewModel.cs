using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeSpeaker.Maui.Models;
using HomeSpeaker.Maui.Services;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class StreamPageViewModel : ObservableObject
{
    private List<StreamModel> AllStreamsList { get; } = new();
    public ObservableCollection<StreamModel> FilteredStreamsList { get; } = new();

    private readonly IPlayerContext _context;
    private readonly INavigationService _navigationService;
    private readonly IMusicStreamService _musicStreamService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public StreamPageViewModel(IPlayerContext playerContext, INavigationService navigationService, IMusicStreamService musicStreamService)
    {
        _context = playerContext;
        _navigationService = navigationService;
        _musicStreamService = musicStreamService;
        LoadStreams();
        FilterStreams();
    }

    private void LoadStreams()
    {
        AllStreamsList.Clear();
        List<string> keys = [.. _musicStreamService.Streams.Keys];
        List<string> values = [.. _musicStreamService.Streams.Values];

        for (int i = 0; i < keys.Count; i++)
        {
            AllStreamsList.Add(new()
            {
                Name = keys[i],
                Url = values[i]
            });
        }
    }

    private void FilterStreams()
    {
        FilteredStreamsList.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchQuery)
            ? AllStreamsList
            : AllStreamsList
                .Where(stream => stream.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

        foreach (var song in filtered)
        {
            FilteredStreamsList.Add(song);
        }
    }

    partial void OnSearchQueryChanged(string value)
    {
        FilterStreams();
    }

    [RelayCommand]
    private async Task NavigateToMainPage()
    {
        await _navigationService.GoToAsync("///MainPage");
    }
}
