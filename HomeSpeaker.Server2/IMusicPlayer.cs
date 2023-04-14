using HomeSpeaker.Shared;

namespace HomeSpeaker.Server;

public interface IMusicPlayer
{
    void PlaySong(Song song);
    void PlayStream(string streamUrl);
    bool StillPlaying { get; }
    void EnqueueSong(Song song);
    PlayerStatus Status { get; }
    IEnumerable<Song> SongQueue { get; }
    void ClearQueue();
    void ResumePlay();
    void SkipToNext();
    void Stop();
    void SetVolume(int level0to100);
    void ShuffleQueue();
    void UpdateQueue(IEnumerable<string> songs);

    event EventHandler<string> PlayerEvent;
}
