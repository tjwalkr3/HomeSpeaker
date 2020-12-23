using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LinuxSoxMusicPlayer> logger;
        private Process playerProcess;

        public LinuxSoxMusicPlayer(ILogger<LinuxSoxMusicPlayer> logger)
        {
            this.logger = logger;
        }

        public void PlaySong(string filePath)
        {
            foreach (var existingVlc in Process.GetProcessesByName("play"))
                existingVlc.Kill();

            playerProcess = new Process();
            playerProcess.StartInfo.FileName = "play";
            playerProcess.StartInfo.Arguments = $"\"{filePath}\"";
            playerProcess.StartInfo.UseShellExecute = false;
            playerProcess.StartInfo.RedirectStandardOutput = true;
            playerProcess.OutputDataReceived += (sender, args) => logger.LogInformation(args.Data);
            playerProcess.Start();
        }

        public bool StillPlaying => playerProcess?.HasExited ?? true == false;
    }
}
