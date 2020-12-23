using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSpeaker.Server
{
    public interface IMusicPlayer
    {
        void PlaySong(string filePath);
        bool StillPlaying { get; }
    }
}
