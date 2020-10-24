using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSpeaker.Lib
{
    public class Song
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
    }
}
