using HomeSpeaker.Shared;
using System.Diagnostics;

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
            killVlc();

            playerProcess = new Process();
            playerProcess.StartInfo.FileName = vlc;
            playerProcess.StartInfo.Arguments = $"\"{filePath}\"";
            playerProcess.StartInfo.UseShellExecute = false;
            playerProcess.StartInfo.RedirectStandardOutput = true;
            playerProcess.OutputDataReceived += (sender, args) => logger.LogInformation(args.Data);
            playerProcess.Start();
        }

        private static void killVlc()
        {
            foreach (var existingVlc in Process.GetProcessesByName("vlc"))
                existingVlc.Kill();
        }

        public void EnqueueSong(string path)
        {
            PlaySong(path);//HACK: This is all messed up.  Copy over the logic from the linux player.
        }

        public void ClearQueue()
        {
            logger.LogError("Windows plaer does not support queuing");
        }

        public void ResumePlay()
        {
            logger.LogError("Windows plaer does not support resuming play");

        }

        public void SkipToNext()
        {
            logger.LogError("Windows plaer does not support skipping to next");
        }

        public void Stop()
        {
            killVlc();
        }

        public void SetVolume(int level)
        {
            logger.LogError("Windows player does not support setting volume");
        }

        public void ShuffleQueue()
        {
            logger.LogError("Windows player does not support shuffling");
        }

        public void PlayStream(string streamUrl)
        {
            logger.LogError("Windows player does not support playing streams");
        }

        public bool StillPlaying => playerProcess?.HasExited ?? true == false;

        public PlayerStatus Status => new PlayerStatus { CurrentSong = new Song { Album = "Unknown", Artist = "Unknown", Name = "Unknown", Path = "Unknown", SongId = 1 } };

        public IEnumerable<Song> SongQueue => throw new NotImplementedException();
    }
}
