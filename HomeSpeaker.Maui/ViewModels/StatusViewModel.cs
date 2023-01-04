using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public partial class StatusViewModel : BaseViewModel
{
    public StatusViewModel(Database database, HomeSpeakerClient client)
    {
        Title = "Status";
        NowPlayingQueue = new ObservableCollection<SongViewModel>();
        this.client = client;
        RefreshStatusCommand.Execute(this);
        this.database = database;
    }

    public string Name { get; set; }
    public string Folder { get; set; }

    private string nowPlayingTitle;
    public string NowPlayingTitle
    {
        get => nowPlayingTitle;
        set { SetProperty(ref nowPlayingTitle, value); }
    }

    private TimeSpan elapsed;
    public TimeSpan Elapsed
    {
        get => elapsed;
        set { SetProperty(ref elapsed, value); }
    }

    private TimeSpan remaining;
    public TimeSpan Remaining
    {
        get => remaining;
        set { SetProperty(ref remaining, value); }
    }

    private double percentComplete;
    public double PercentComplete
    {
        get => percentComplete;
        set { SetProperty(ref percentComplete, value); }
    }
    private ObservableCollection<SongViewModel> queue;

    public string Title { get; }
    public ObservableCollection<SongViewModel> NowPlayingQueue
    {
        get => queue;
        set => SetProperty(ref queue, value);
    }
    private Command stop;
    public Command Stop => stop ??= new Command(() =>
    {
        client.PlayerControl(new PlayerControlRequest { Stop = true });
        RefreshStatusCommand.Execute(this);
    });
    private Command play;
    public Command Play => play ??= new Command(() =>
    {
        client.PlayerControl(new PlayerControlRequest { Play = true });
        RefreshStatusCommand.Execute(this);
    });
    private Command forward;
    public Command Forward => forward ??= new Command(() =>
    {
        client.PlayerControl(new PlayerControlRequest { SkipToNext = true });
        RefreshStatusCommand.Execute(this);
    });
    private Command clear;
    public Command Clear => clear ??= new Command(() =>
    {
        client.PlayerControl(new PlayerControlRequest { ClearQueue = true });
        RefreshStatusCommand.Execute(this);
    });
    private string exception;
    public string Exception
    {
        get => exception;
        set { SetProperty(ref exception, value); }
    }
    private int queueLength;
    public int QueueLength
    {
        get => queueLength;
        set { SetProperty(ref queueLength, value); }
    }
    private Command shuffle;
    public Command Shuffle => shuffle ??= new Command(() =>
    {
        client.ShuffleQueue(new ShuffleQueueRequest());
        RefreshStatusCommand.Execute(this);
    });

    private readonly HomeSpeakerClient client;
    private readonly Database database;
    [ObservableProperty]
    private bool isBusy;

    private Command refreshStatusCommand;
    public Command RefreshStatusCommand => refreshStatusCommand ??= new Command(async () =>
    {
        IsBusy = true;
        try
        {
            NowPlayingQueue.Clear();
            var getQueueReply = client.GetPlayQueue(new GetSongsRequest());
            await foreach (var reply in getQueueReply.ResponseStream.ReadAllAsync())
            {
                foreach (var s in from songMessage in reply.Songs
                                  select songMessage.ToSongViewModel(database, client))
                {
                    NowPlayingQueue.Add(s);
                }
            }
            QueueLength = NowPlayingQueue.Count;
            OnPropertyChanged(nameof(NowPlayingQueue));

            var statusReply = client.GetPlayerStatus(new GetStatusRequest());
            NowPlayingTitle = statusReply.CurrentSong.Name;
            Elapsed = statusReply.Elapsed.ToTimeSpan();
            Remaining = statusReply.Remaining.ToTimeSpan();
            PercentComplete = statusReply.PercentComplete;
            Exception = null;
        }
        catch (Exception ex)
        {
            Exception = ex.ToString();
        }
        finally
        {
            IsBusy = false;
        }
    });

    public void OnAppearing()
    {
        IsBusy = true;
    }

    //async void OnItemSelected(Item item)
    //{
    //    if (item == null)
    //        return;

    //    // This will push the ItemDetailPage onto the navigation stack
    //    await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
    //}
}