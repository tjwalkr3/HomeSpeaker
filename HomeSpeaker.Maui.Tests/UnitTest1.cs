using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Maui.Tests;

public class FilterTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task FilterSongs()
    {
        //Arrange
        var staredSongMock = new Mock<IStaredSongDb>();
        var playerServiceMock = new Mock<IPlayerService>();
        var loggerMock = new Mock<ILogger<FoldersViewModel>>();
        var folder1Songs = new List<SongViewModel>()
        {
            new SongViewModel
            {
                Album = "Album1",
                Artist = "Artist1",
                Folder = "Folder1",
                Name = "Song1",
                Path = "Folder1/Song1.mp3",
                SongId = 0
            }
        };
        var folder2Songs = new List<SongViewModel>()
        {
            new SongViewModel
            {
                Album = "Album2",
                Artist = "Artist2",
                Folder = "Folder2",
                Name = "Song2",
                Path = "Folder2/Song2.mp3",
                SongId = 0
            }
        };

        playerServiceMock.Setup(m => m.GetSongGroups()).ReturnsAsync(() =>
        {
            var groups = new Dictionary<string, List<SongViewModel>>
            {
                { "Folder1", folder1Songs },
                { "Folder2", folder2Songs }
            };
            return groups;
        });

        var vm = new FoldersViewModel(staredSongMock.Object, playerServiceMock.Object, loggerMock.Object);
        await vm.Loading();

        //Act
        vm.FilterText = "1";
        vm.PerformFilterCommand.Execute(this);

        //Assert
        vm.FilteredSongs.Count().Should().Be(1);
    }
}