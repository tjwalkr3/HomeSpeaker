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
            playerProcess.StartInfo.RedirectStandardError = true;

            playerProcess.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                try
                {
                    var parts = e.Data.Split(new char[] { ' ', '%', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    var percentComplete = decimal.Parse(parts[0])/100;
                    var elapsedTime = parts[1];
                    var remainingTime = parts[2];
                    logger.LogInformation($"Elapsed: {elapsedTime}; Remaining: {remainingTime}; Percent Complete: {percentComplete:p}");
                }
                catch
                {
                    logger.LogInformation(e.Data);
                }
            });
            playerProcess.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                logger.LogError(e.Data);
            });

            logger.LogInformation($"Starting to play {filePath}");
            playerProcess.Start();

            playerProcess.BeginOutputReadLine();
            playerProcess.BeginErrorReadLine();
            //string outputLine;
            //while((outputLine = await playerProcess.StandardOutput.Read()) != null)
            //{
            //    logger.LogInformation(outputLine);
            //}
        }

        public bool StillPlaying => playerProcess?.HasExited ?? true == false;
    }
}
