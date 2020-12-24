using HomeSpeaker.Shared;
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
        private readonly Mp3Library library;
        private Process playerProcess;

        public LinuxSoxMusicPlayer(ILogger<LinuxSoxMusicPlayer> logger, Mp3Library library)
        {
            this.logger = logger;
            this.library = library;
        }

        private PlayerStatus status;
        private Song currentSong;
        public PlayerStatus Status => (status ?? new PlayerStatus()) with { CurrentSong = currentSong };

        private bool startedPlaying = false;

        public void PlaySong(string filePath)
        {
            startedPlaying = true;
            currentSong = library.Songs.Single(s => s.Path == filePath);
            stopPlaying();
            stoppedSong = null;

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
                    this.status = status;
                }
                else
                {
                    this.status = new PlayerStatus();
                }
            });
            playerProcess.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (TryParsePlayerOutput(e.Data, out var status))
                {
                    this.status = status;
                }
                else
                {
                    this.status = new PlayerStatus();
                }
            });

            logger.LogInformation($"Starting to play {filePath}");
            playerProcess.EnableRaisingEvents = true;
            playerProcess.Start();
            playerProcess.Exited += PlayerProcess_Exited;

            playerProcess.BeginOutputReadLine();
            playerProcess.BeginErrorReadLine();
            startedPlaying = false;
        }

        private static void stopPlaying()
        {
            foreach (var existingVlc in Process.GetProcessesByName("play"))
                existingVlc.Kill();
        }

        private void PlayerProcess_Exited(object sender, EventArgs e)
        {
            logger.LogInformation("Finished playing a song.");
            currentSong = null;
            if (songQueue.Count > 0)
            {
                playNextSongInQueue();
            }
            else
            {
                logger.LogInformation("Nothing in the queue, so Status is now empty.");
                status = new PlayerStatus();
            }
        }

        private void playNextSongInQueue()
        {
            logger.LogInformation($"There are still {songQueue.Count} songs in the queue, so I'll play the next one:");
            if (songQueue.TryDequeue(out var nextSong))
            {
                PlaySong(nextSong.Path);
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
                playerStatus = new PlayerStatus();
                return false;
            }
        }

        public void EnqueueSong(string path)
        {
            var story = new StringBuilder($"Queuing up {path}\n");

            if (StillPlaying)
            {
                story.AppendLine("StillPlaying is true, so I'll add to queue.");
                var song = library.Songs.Single(s => s.Path == path);
                songQueue.Enqueue(song);
                story.AppendLine($"Added song# {song.SongId} to queue, now contains {songQueue.Count} songs.");
            }
            else
            {
                story.AppendLine("Nothing playing, so instead of queuing I'll just play it...");
                PlaySong(path);
            }
            logger.LogInformation(story.ToString());
        }

        public void ClearQueue()
        {
            songQueue.Clear();
        }

        public void ResumePlay()
        {
            if (StillPlaying == false && stoppedSong != null)
            {
                PlaySong(stoppedSong.Path);
            }
            else if (songQueue.Any())
            {
                playNextSongInQueue();
            }
        }

        public void SkipToNext()
        {
            Stop();
            playNextSongInQueue();
        }

        private Song stoppedSong;

        public void Stop()
        {
            stoppedSong = currentSong;
            stopPlaying();
        }

        public void SetVolume(int level0to100)
        {
            int actualMin = 40;
            int actualMax = 100;
            var percent = Math.Max(0, Math.Min(100, level0to100)) / 100M;
            var newLevel = (actualMax - actualMin) * percent + actualMin;
            logger.LogInformation("Desired volume: {level0to100}; newLevel {newLevel} = (actualMax {actualMax} - actual Min {actualMin}) * percent {percent} + actualMin {actualMin}",
                level0to100, newLevel, actualMax, actualMin, percent, actualMin);
            Process.Start("amixer", $"--card 0 sset Headphone,0 {newLevel}%");
        }

        public bool StillPlaying
        {
            get
            {
                logger.LogInformation($"StillPlaying: startedPlaying {startedPlaying} || (playerProcess?.HasExited {playerProcess?.HasExited} ?? true) {playerProcess?.HasExited ?? false} == false) {(playerProcess?.HasExited ?? true) == false}");
                return startedPlaying || (playerProcess?.HasExited ?? true) == false;
            }
        }

        private ConcurrentQueue<Song> songQueue = new ConcurrentQueue<Song>();

        public IEnumerable<Song> SongQueue => songQueue.ToArray();
    }
}
