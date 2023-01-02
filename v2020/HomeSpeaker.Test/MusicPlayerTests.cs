using FluentAssertions;
using HomeSpeaker.Server;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Test
{
    public class MusicPlayerTests
    {
        [Test]
        public void ParsePlayback()
        {
            var playerOutput = "In:1.11% 00:00:03.81 [00:05:39.80] Out:168k  [ =====|===== ] Hd:3.2 Clip:0";
            var parseResult = LinuxSoxMusicPlayer.TryParsePlayerOutput(playerOutput, out var status);
            parseResult.Should().BeTrue();
            status.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds( 3), TimeSpan.FromMilliseconds( 810));
            status.Remaining.Should().BeCloseTo(new TimeSpan(0, 0, 5, 39), TimeSpan.FromMilliseconds( 800));
            status.PercentComplete.Should().Be(.0111M);
            status.StillPlaying.Should().BeTrue();
        }
    }
}
