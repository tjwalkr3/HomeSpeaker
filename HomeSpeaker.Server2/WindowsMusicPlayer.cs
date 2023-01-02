using HomeSpeaker.Shared;
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
    public class WindowsMusicPlayer : IMusicPlayer
    {
        public WindowsMusicPlayer(ILogger<WindowsMusicPlayer> logger)
        {
            this.logger = logger;
        }

        const string vlc = @"c:\program files\videolan\vlc\vlc.exe";
        private readonly ILogger<WindowsMusicPlayer> logger;
        private Process playerProcess;

        public void PlaySong(string filePath)
        {
            foreach (var existingVlc in Process.GetProcessesByName("vlc"))
                existingVlc.Kill();

            playerProcess = new Process();
            playerProcess.StartInfo.FileName = vlc;
            playerProcess.StartInfo.Arguments = $"\"{filePath}\"";
            playerProcess.StartInfo.UseShellExecute = false;
            playerProcess.StartInfo.RedirectStandardOutput = true;
            playerProcess.OutputDataReceived += (sender, args) => logger.LogInformation(args.Data);
            playerProcess.Start();
        }

        public void EnqueueSong(string path)
        {
            logger.LogError("Windows player does not support queuing");
        }

        public void ClearQueue()
        {
            throw new NotImplementedException();
        }

        public void ResumePlay()
        {
            throw new NotImplementedException();
        }

        public void SkipToNext()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int level) => throw new NotImplementedException();

        public void ShuffleQueue()
        {
            throw new NotImplementedException();
        }

        public void PlayStream(string streamUrl)
        {
            throw new NotImplementedException();
        }

        public bool StillPlaying => playerProcess?.HasExited ?? true == false;

        public PlayerStatus Status => throw new NotImplementedException();

        public IEnumerable<Song> SongQueue => throw new NotImplementedException();
    }
}
