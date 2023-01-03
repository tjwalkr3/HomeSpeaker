using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui.ViewModels;

public class StreamViewModel : BaseViewModel
{
    public string Title { get; }

    private HomeSpeakerClient client;

    public StreamViewModel(HomeSpeakerClient client)
    {
        Title = "Internet Radio Streams";
        this.client = client;
    }

    public Dictionary<string, string> Streams { get; private set; } = new()
    {
        { "KBAQ", "https://kbaq.streamguys1.com/kbaq_mp3_128" },
        { "Your Classical Radio", "https://ycradio.stream.publicradio.org/ycradio.aac" },
        { "YourClassical MPR", "https://cms.stream.publicradio.org/cms.aac" },
        { "Relax Radio", "https://relax.stream.publicradio.org/relax.aac" },
        { "Lullabies", "https://lullabies.stream.publicradio.org/lullabies.aac" },
        { "Choral Stream", "https://choral.stream.publicradio.org/choral.aac" },
        { "Classical Favorites", "https://favorites.stream.publicradio.org/favorites.aac" },
        { "Chamber Music", "https://chambermusic.stream.publicradio.org/chambermusic.aac" },
        { "Peaceful Piano", "https://peacefulpiano.stream.publicradio.org/peacefulpiano.aac" },
        { "Hygge", "https://hygge.stream.publicradio.org/hygge.aac" },
        { "Focus", "https://focus.stream.publicradio.org/focus.aac" },
        { "Concert Band", "https://concertband.stream.publicradio.org/concertband.aac" },
        { "Classical Kids", "https://classicalkids.stream.publicradio.org/classicalkids.aac" },
        { "Holiday Stream", "https://holiday.stream.publicradio.org/holiday_yc.aac" },
        { "Classical 89", "https://radio.byub.org/classical89/classical89_aac" }
    };

    private Command<string> playStream;
    public Command<string> PlayStream => playStream ??= new Command<string>(async (path) =>
        await client.PlayStreamAsync(new PlayStreamRequest { StreamUrl = path })
    );
}