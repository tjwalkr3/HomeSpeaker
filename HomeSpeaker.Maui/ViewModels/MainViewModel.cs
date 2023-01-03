using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly HomeSpeakerClient client;

    public MainViewModel(HomeSpeakerClient client)
    {
        this.client = client;
    }

    [RelayCommand]
    public async Task ViewModelLoading()
    {
        if (songs.Count > 0)
            return;

        try
        {
            var getSongsReply = client.GetSongs(new GetSongsRequest { });
            await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
            {
                var newSongs = reply.Songs.Select(s => s.ToSong());
                foreach (var song in newSongs)
                {
                    songs.Add(song);
                    if (songs.Count > 100)
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.ToString();
        }
    }

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private ObservableCollection<Song> songs = new();
}
