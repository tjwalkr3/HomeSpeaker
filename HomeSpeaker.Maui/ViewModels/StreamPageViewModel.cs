using CommunityToolkit.Mvvm.ComponentModel;
using HomeSpeaker.Maui.Services;
using System.Collections.ObjectModel;
namespace HomeSpeaker.Maui.ViewModels;

public partial class StreamPageViewModel : ObservableObject
{
    public ObservableCollection<MusicStream> Streams { get; set; }

    private readonly IPlayerContext _context;
    private readonly INavigationService _navigationService;

    public StreamPageViewModel(IPlayerContext playerContext, INavigationService navigationService)
    {
        _context = playerContext;
        _navigationService = navigationService;
    }

    private void LoadStreams()
    {
        Streams.Clear();

    }
}
