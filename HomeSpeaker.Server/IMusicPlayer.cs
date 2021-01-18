using HomeSpeaker.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Server
{
    public interface IMusicPlayer
    {
        void PlaySong(string filePath);
        bool StillPlaying { get; }
        void EnqueueSong(string path);
        PlayerStatus Status { get; }
        IEnumerable<Song> SongQueue { get; }
        void ClearQueue();
        void ResumePlay();
        void SkipToNext();
        void Stop();
        void SetVolume(int level0to100);
        void ShuffleQueue();
    }
}
