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

        public async Task PlaySongAsync(string filePath)
        {
            foreach (var existingVlc in Process.GetProcessesByName("play"))
                existingVlc.Kill();

            playerProcess = new Process();
            playerProcess.StartInfo.FileName = "play";
            playerProcess.StartInfo.Arguments = $"\"{filePath}\"";
            playerProcess.StartInfo.UseShellExecute = false;
            playerProcess.StartInfo.RedirectStandardOutput = true;

            logger.LogInformation($"Starting to play {filePath}");
            playerProcess.Start();
            //playerProcess.BeginOutputReadLine();
            string outputLine;
            while((outputLine = await playerProcess.StandardOutput.ReadLineAsync()) != null)
            {
                logger.LogInformation(outputLine);
            }
        }

        public bool StillPlaying => playerProcess?.HasExited ?? true == false;
    }
}
