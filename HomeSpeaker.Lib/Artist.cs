using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeSpeaker.Lib
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public IQueryable<Album> Albums { get; set; }
        public IQueryable<Song> Songs { get; set; }
    }
}
