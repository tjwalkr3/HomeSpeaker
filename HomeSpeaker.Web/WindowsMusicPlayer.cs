using HomeSpeaker.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeSpeaker.Web
{
    public class WindowsMusicPlayer : IMusicPlayer
    {
        const string vlc = @"c:\program files\videolan\vlc\vlc.exe";

        public void PlaySong(string filePath)
        {
            foreach (var existingVlc in Process.GetProcessesByName("vlc"))
                existingVlc.Kill();

            //Process.Start(vlc, $"\"{filePath}\"");

  
        }
    }
}
