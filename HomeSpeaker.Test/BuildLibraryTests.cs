using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using HomeSpeaker.Shared;
using HomeSpeaker.Server;

namespace HomeSpeaker.Test
{
    public class BuildLibraryTests
    {
        [SetUp]
        public void Setup()
        {

        }

        IFileSource fileSource;
        IDataStore dataStore;
        ITagParser tagParser;


        [Test]
        public void ASongShowsUp()
        {
            var songs = new List<Song>();
            var expectedSong = new Song
            {
                SongId = 1,
                Artist = "TestArtist",
                Album = "TestAlbum",
                Name = "TestSong",
                Path = "TestArtist/TestAlbum/TestSong.mp3"
            };
            var fsMock = new Mock<IFileSource>();
            fsMock.Setup(m => m.GetAllMp3s()).Returns(new[] { new FileInfo("bogus.mp3") });
            fileSource = fsMock.Object;

            var dsMock = new Mock<IDataStore>();
            dsMock.Setup(m => m.AddOrUpdateAsync(It.IsAny<Song>())).Callback<Song>(s => songs.Add(s));
            dsMock.Setup(m => m.GetSongs()).Returns(songs);
            dataStore = dsMock.Object;

            var tpMock = new Mock<ITagParser>();
            tpMock.Setup(m => m.CreateSong(It.IsAny<FileInfo>())).Returns(new Song
            {
                SongId = 1,
                Artist = "TestArtist",
                Album = "TestAlbum",
                Name = "TestSong",
                Path = "TestArtist/TestAlbum/TestSong.mp3"
            });
            tagParser = tpMock.Object;

            var loggerMock = new Mock<ILogger<Mp3Library>>();

            var library = new Mp3Library(fileSource, tagParser, dataStore, loggerMock.Object);
            while(Mp3Library.SyncCompleted is false)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(250));
            }
            library.Songs.Single().Should().BeEquivalentTo(expectedSong, options =>
                options.AllowingInfiniteRecursion()
            );
        }

        //[Test]
        //public void TwoSongsFromSameArtistCreateTwoSongsButOneArtist()
        //{
        //    var songs = new List<Song>();
        //    var expectedSong = new Song
        //    {
        //        SongId = 1,
        //        Artist = "TestArtist",
        //        Album = "TestAlbum",
        //        Name = "TestSong",
        //        Path = "TestArtist/TestAlbum/TestSong.mp3"
        //    };
        //    var expectedSong2 = new Song
        //    {
        //        SongId = 1,
        //        Artist = "TestArtist",
        //        Album = "TestAlbum",
        //        Name = "TestSong2",
        //        Path = "TestArtist/TestAlbum/TestSong2.mp3"
        //    };
        //    var fsMock = new Mock<IFileSource>();
        //    fsMock.Setup(m => m.GetAllMp3s()).Returns(new[] { "bogus", "bgus2" });
        //    fileSource = fsMock.Object;

        //    var dsMock = new Mock<IDataStore>();
        //    dsMock.Setup(m => m.AddOrUpdate(It.IsAny<Song>())).Callback<Song>(s => songs.Add(s));
        //    dsMock.Setup(m => m.GetSongs()).Returns(songs);
        //    dataStore = dsMock.Object;

        //    var tpMock = new Mock<ITagParser>();
        //    tpMock.Setup(m => m.CreateSong(It.IsAny<object>())).Returns(new Song
        //    {
        //        SongId = 1,
        //        Artist = "TestArtist",
        //        Album = "TestAlbum",
        //        Name = "TestSong",
        //        Path = "TestArtist/TestAlbum/TestSong.mp3"
        //    });
        //    tagParser = tpMock.Object;

        //    var library = new Mp3Library(fileSource, tagParser, dataStore);
        //    Task.WaitAll(library.LibrarySyncTask);
        //    library.Songs.Single().Should().BeEquivalentTo(expectedSong, options =>
        //        options.AllowingInfiniteRecursion()
        //    );
        //}
    }
}