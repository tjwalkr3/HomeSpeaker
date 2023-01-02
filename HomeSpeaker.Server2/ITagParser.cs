using HomeSpeaker.Shared;
using Id3;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            var mp3 = new Mp3(file); var tag = mp3.GetTag(Id3TagFamily.Version2X) ?? mp3.GetTag(Id3TagFamily.Version1X) ?? throw new ApplicationException("Unable to find MP3 tags for " + file.FullName);
            return new Song
            {
                Album = tag.Album.Value,
                Artist = tag.Artists.Value.FirstOrDefault() ?? "[Artist Unknown]",
                Name = tag.Title.Value,
                Path = file.FullName
            };
        }
    }
}
