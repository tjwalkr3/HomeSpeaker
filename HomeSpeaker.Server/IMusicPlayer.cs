using HomeSpeaker.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Server
{
    public interface IMusicPlayer
    {
        Task PlaySongAsync(string filePath);
        bool StillPlaying { get; }
        void EnqueueSong(string path);
        PlayerStatus Status { get; }
        IEnumerable<Song> SongQueue { get; }
    }
}
