using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSpeaker.Mobile.Models
{
    public class StarredSong
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Path { get; set; }
    }
}
