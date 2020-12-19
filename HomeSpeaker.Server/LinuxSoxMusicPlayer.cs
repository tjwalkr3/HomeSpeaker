using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeSpeaker.Server
{
    public class LinuxSoxMusicPlayer : IMusicPlayer
    {

        public void PlaySong(string filePath)
        {
            foreach (var existingVlc in Process.GetProcessesByName("play"))
                existingVlc.Kill();

            Process.Start("play", $"\"{filePath}\"");

  
        }
    }
}
