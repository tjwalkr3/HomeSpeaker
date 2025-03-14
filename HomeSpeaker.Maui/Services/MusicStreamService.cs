﻿namespace HomeSpeaker.Maui.Services;

public class MusicStreamService
{
    public Dictionary<string, string> _streams { get; private set; }

    public MusicStreamService()
    {
        _streams = new Dictionary<string, string>
        {
            {"Church Music Stream", "https://nmcdn-lds.msvdn.net/icecastRelay/101156/GvaVK70/icecast?rnd=637109878513586401"},
            {"Tabernacle Choir Stream", "https://nmcdn-lds.msvdn.net/icecastRelay/101158/3nGepF3/icecast?rnd=637109879815090752"},
            {"Canal Espanol", "https://nmcdn-lds.msvdn.net/icecastRelay/101157/V2Pm3WE/icecast?rnd=637109879429639917"},
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
            { "Classical 89", "https://radio.byub.org/classical89/classical89_aac" },
            { "Guitar Stream", "https://guitar.stream.publicradio.org/guitar.aac" },
            { "Madrigals", "https://streams.calmradio.com:14228/stream" },
            { "Radio Clasique", "https://listen.radioking.com/radio/228241/stream/271810" },
            { "Positively Baroque", "https://play.organlive.com:7020/128" },
            { "Adagio Radio (Madrid)", "https://stream.tunerplay.com/listen/adagioradio/adagioradio.mp3" }
        };
    }

    public bool AddStream(string key, string value)
    {
        return _streams.TryAdd(key, value);
    }

    public bool RemoveStream(string key)
    {
        return _streams.Remove(key);
    }
}