using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeSpeaker.Lib;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeSpeaker.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly Mp3Library mp3Library;
        private readonly IMusicPlayer musicPlayer;

        public MusicController(Mp3Library mp3Library, IMusicPlayer musicPlayer)
        {
            this.mp3Library = mp3Library;
            this.musicPlayer = musicPlayer;
        }

        // GET: api/<MusicController>
        [HttpGet]
        public IEnumerable<Song> Get()
        {
            return mp3Library.Songs;
        }

        // GET api/<MusicController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var song = mp3Library.Songs.First(s => s.SongId == id);
            if (song == null)
                return "No found";
            musicPlayer.PlaySong(song.Path);
            return "ok";
        }

        // POST api/<MusicController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MusicController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MusicController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
