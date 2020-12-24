using HomeSpeaker.Server.gRPC;
using HomeSpeaker.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Client
{
    public static class ProtobufExtensions
    {
        public static PlayerStatus ToPlayerStatus(this GetStatusReply reply)
        {
            return new PlayerStatus
            {
                CurrentSong = reply.CurrentSong.ToSong(),
                Elapsed = reply.Elapsed.ToTimeSpan(),
                PercentComplete = (decimal)reply.PercentComplete,
                Remaining = reply.Remaining.ToTimeSpan(),
                StillPlaying = reply.StilPlaying
            };
        }

        public static Song ToSong(this Server.gRPC.SongMessage song)
        {
            return new Song
            {
                SongId = song?.SongId ?? -1,
                Name = song?.Name ?? "[ Null Song Response ??? ]",
                Album = song?.Album,
                Artist = song?.Artist,
                Path = song?.Path
            };
        }
    }
}
