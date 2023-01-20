using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HomeSpeaker.Shared;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Server
{
    public class HomeSpeakerService : HomeSpeakerBase
    {
        private readonly ILogger<HomeSpeakerService> logger;
        private readonly Mp3Library library;
        private readonly IMusicPlayer musicPlayer;

        public HomeSpeakerService(ILogger<HomeSpeakerService> logger, Mp3Library library, IMusicPlayer musicPlayer)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.library = library ?? throw new System.ArgumentNullException(nameof(library));
            this.musicPlayer = musicPlayer ?? throw new System.ArgumentNullException(nameof(musicPlayer));
        }

        public override async Task GetSongs(GetSongsRequest request, IServerStreamWriter<GetSongsReply> responseStream, ServerCallContext context)
        {
            var reply = new GetSongsReply();
            if (library?.Songs?.Any() ?? false)
            {
                IEnumerable<Song> songs = library.Songs;
                if (!string.IsNullOrEmpty(request.Folder))
                {
                    songs = songs.Where(s => s.Path.Contains(request.Folder));
                }
                logger.LogInformation("Found songs!  Sending to client.");
                reply.Songs.AddRange(translateSongs(songs));
            }
            else
            {
                logger.LogInformation("No songs found.  Sending back empty list.");
            }
            await responseStream.WriteAsync(reply);
        }

        public override Task<PlaySongReply> PlaySong(PlaySongRequest request, ServerCallContext context)
        {
            var song = library.Songs.FirstOrDefault(s => s.SongId == request.SongId);
            var reply = new PlaySongReply { Ok = false };
            if (song != null)
            {
                Task.Run(() =>
                    musicPlayer.PlaySong(song)
                );
                reply.Ok = true;
            }
            return Task.FromResult(reply);
        }

        public override Task<PlaySongReply> PlayStream(PlayStreamRequest request, ServerCallContext context)
        {
            musicPlayer.PlayStream(request.StreamUrl);
            return Task.FromResult(new PlaySongReply { Ok = true });
        }

        public override Task<Empty> ResetLibrary(Empty request, ServerCallContext context)
        {
            library.ResetLibrary();
            return Task.FromResult(new Empty());
        }

        public override Task<PlaySongReply> EnqueueSong(PlaySongRequest request, ServerCallContext context)
        {
            var song = library.Songs.FirstOrDefault(s => s.SongId == request.SongId);
            var reply = new PlaySongReply { Ok = false };
            if (song != null)
            {
                logger.LogInformation($"Queuing up #{song.SongId}: {song.Name}");
                musicPlayer.EnqueueSong(song);
                reply.Ok = true;
            }
            return Task.FromResult(reply);
        }

        public override Task<GetStatusReply> GetPlayerStatus(GetStatusRequest request, ServerCallContext context)
        {
            var status = musicPlayer?.Status ?? new Shared.PlayerStatus();
            return Task.FromResult(new GetStatusReply
            {
                Elapsed = Duration.FromTimeSpan(status.Elapsed),
                PercentComplete = (double)status.PercentComplete,
                Remaining = Duration.FromTimeSpan(status.Remaining),
                StilPlaying = status.StillPlaying,
                CurrentSong = translateSong(status.CurrentSong)
            });
        }

        public override async Task GetPlayQueue(GetSongsRequest request, IServerStreamWriter<GetSongsReply> responseStream, ServerCallContext context)
        {
            var reply = new GetSongsReply();
            System.Collections.Generic.IEnumerable<Shared.Song> songQueue = musicPlayer.SongQueue;
            if (songQueue.Any())
            {
                logger.LogInformation("Found songs in queue!  Sending to client.");
                reply.Songs.AddRange(translateSongs(songQueue));
            }
            else
            {
                logger.LogInformation("No songs in queue.  Sending back empty list.");
            }
            await responseStream.WriteAsync(reply);
        }

        private IEnumerable<SongMessage> translateSongs(IEnumerable<Song> songQueue)
        {
            return songQueue.Select(translateSong);
        }

        private SongMessage translateSong(Song s)
        {
            return new SongMessage
            {
                Album = s?.Album ?? "[ No Album ]",
                Artist = s?.Artist ?? "[ No Artist ]",
                Name = s?.Name ?? Path.GetFileNameWithoutExtension(s?.Path),
                Path = s?.Path.Substring(s.Path.IndexOf(library.RootFolder) + library.RootFolder.Length),
                SongId = s?.SongId ?? -1
            };
        }

        public override Task<PlayerControlReply> PlayerControl(PlayerControlRequest request, ServerCallContext context)
        {
            if (request.ClearQueue)
            {
                musicPlayer.ClearQueue();
            }
            if (request.Play)
            {
                musicPlayer.ResumePlay();
            }
            if (request.SkipToNext)
            {
                musicPlayer.SkipToNext();
            }
            if (request.Stop)
            {
                musicPlayer.Stop();
            }
            if (request.SetVolume)
            {
                musicPlayer.SetVolume(request.VolumeLevel);
            }
            return Task.FromResult(new PlayerControlReply());
        }

        public override Task<ShuffleQueueReply> ShuffleQueue(ShuffleQueueRequest request, ServerCallContext context)
        {
            musicPlayer.ShuffleQueue();
            return Task.FromResult(new ShuffleQueueReply());
        }
    }
}
