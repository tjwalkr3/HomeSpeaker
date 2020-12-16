using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Lib
{
    public class Mp3Library
    {
        private readonly IFileSource fileSource;
        private readonly ITagParser tagParser;
        private readonly IDataStore dataStore;
        private readonly ILogger<Mp3Library> logger;

        public Mp3Library(IFileSource fileSource, ITagParser tagParser, IDataStore dataStore, ILogger<Mp3Library> logger)
        {
            this.fileSource = fileSource ?? throw new ArgumentNullException(nameof(fileSource));
            this.tagParser = tagParser ?? throw new ArgumentNullException(nameof(tagParser));
            this.dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            logger.LogDebug($"Initialized with fileSource {fileSource.RootFolder}");

            if(SyncStarted is false)
            {
                SyncStarted = true;
                SyncLibrary();
            }
        }

        public static bool SyncStarted { get; private set; }
        public static bool SyncCompleted { get; private set; }

        private async Task SyncLibrary()
        {
            foreach(var file in fileSource.GetAllMp3s())
            {
                var song = tagParser.CreateSong(file);
                await dataStore.AddOrUpdateAsync(song);
            }
            SyncCompleted = true;
        }

        public IEnumerable<Artist> Artists => dataStore.GetArtists();
        public IEnumerable<Album> Albums => dataStore.GetAlbums();
        public IEnumerable<Song> Songs => dataStore.GetSongs();
    }
}
