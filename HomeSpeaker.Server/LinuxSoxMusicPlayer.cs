using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
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
                if (TryParsePlayerOutput(e.Data, out var status))
                {
                    logger.LogInformation(status.ToString());
                }
            });
            playerProcess.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (TryParsePlayerOutput(e.Data, out var status))
                {
                    logger.LogInformation(status.ToString());
                }
            });

            logger.LogInformation($"Starting to play {filePath}");
            playerProcess.Start();
            playerProcess.Exited += PlayerProcess_Exited;

            playerProcess.BeginOutputReadLine();
            playerProcess.BeginErrorReadLine();
        }

        private void PlayerProcess_Exited(object sender, EventArgs e)
        {
            if(songQueue.Count > 0)
            {
                if(songQueue.TryDequeue(out string nextSong))
                {
                    PlaySongAsync(nextSong);
                }
            }
        }

        public static bool TryParsePlayerOutput(string output, out PlayerStatus playerStatus)
        {
            try
            {
                var parts = output.Split(new char[] { ' ', '%', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                var percentComplete = decimal.Parse(parts[0].Substring(parts[0].IndexOf(':') + 1)) / 100;
                var elapsedTime = TimeSpan.Parse(parts[1]);
                var remainingTime = TimeSpan.Parse(parts[2]);
                playerStatus = new PlayerStatus
                {
                    Elapsed = elapsedTime,
                    PercentComplete = percentComplete,
                    Remaining = remainingTime,
                    StillPlaying = true
                };
                return true;
            }
            catch
            {
                playerStatus = null;
                return false;
            }
        }

        public void EnqueueSong(string path)
        {
            if (StillPlaying)
            {
                songQueue.Enqueue(path);
            }
            else
            {
                PlaySongAsync(path);
            }
        }

        public bool StillPlaying => playerProcess?.HasExited ?? true == false;

        private ConcurrentQueue<string> songQueue = new ConcurrentQueue<string>();
    }
    public record PlayerStatus
    {
        public decimal PercentComplete { get; init; }
        public TimeSpan Elapsed { get; init; }
        public TimeSpan Remaining { get; init; }
        public bool StillPlaying { get; init; }
    }
}
