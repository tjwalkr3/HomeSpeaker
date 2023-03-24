using HomeSpeaker.Shared;
using Id3;

namespace HomeSpeaker.Server
{
    public interface ITagParser
    {
        Song CreateSong(FileInfo file);
    }

    public class DefaultTagParser : ITagParser
    {
        private readonly ILogger<DefaultTagParser> logger;

        public DefaultTagParser(ILogger<DefaultTagParser> logger)
        {
            this.logger = logger;
        }

        public Song CreateSong(FileInfo file)
        {
            var mp3 = new Mp3(file);
            var tag = mp3.GetTag(Id3TagFamily.Version2X) ?? mp3.GetTag(Id3TagFamily.Version1X) ?? throw new ApplicationException("Unable to find MP3 tags for " + file.FullName);
            var title = tag.Title?.Value?.Replace("\0", string.Empty) ?? string.Empty;
            if (title.Length == 0)
            {
                title = file.Name.Replace(".mp3", string.Empty);
            }
            return new Song
            {
                Album = tag.Album.Value?.Replace("\0", string.Empty),
                Artist = tag.Artists.Value.FirstOrDefault()?.Replace("\0", string.Empty) ?? "[Artist Unknown]",
                Name = title,
                Path = file.FullName
            };
        }
    }
}
