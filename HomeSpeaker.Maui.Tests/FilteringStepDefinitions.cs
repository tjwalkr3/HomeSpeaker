using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;

namespace HomeSpeaker.Maui.Tests
{
    [Binding]
    public class FilteringStepDefinitions
    {
        private readonly ScenarioContext context;

        public FilteringStepDefinitions(ScenarioContext context)
        {
            this.context = context;

            var allSongs = new Dictionary<string, List<SongViewModel>>();

            var staredSongMock = new Mock<IStaredSongDb>();
            var playerServiceMock = new Mock<IPlayerService>();
            playerServiceMock.Setup(m => m.GetSongGroups()).ReturnsAsync(() => allSongs);

            var loggerMock = new Mock<ILogger<FoldersViewModel>>();
            var vm = new FoldersViewModel(staredSongMock.Object, playerServiceMock.Object, loggerMock.Object);

            context.Set(allSongs);
            context.Set(vm);
        }

        [Given(@"the following songs in the (.*) folder")]
        public void GivenTheFollowingSongsInTheFolderFolder(string folderName, Table table)
        {
            var songsInFolder = new List<SongViewModel>();
            foreach (var row in table.Rows)
            {
                songsInFolder.Add(new SongViewModel
                {
                    Album = row["Album"],
                    Artist = row["Artist"],
                    Folder = folderName,
                    Name = row["Name"],
                    Path = $"{folderName}/{row["Name"]}.mp3",
                    SongId = songsInFolder.Count
                });
            }

            var allSongs = context.Get<Dictionary<string, List<SongViewModel>>>();
            allSongs.Add(folderName, songsInFolder);
        }


        [When(@"I set the filter text to (.*)")]
        public void WhenISetTheFilterTextTo(string filterValue)
        {
            var vm = context.Get<FoldersViewModel>();
            vm.FilterText = filterValue;
        }

        [When(@"click the Filter button")]
        public void WhenClickTheFilterButton()
        {
            var vm = context.Get<FoldersViewModel>();
            vm.PerformFilterCommand.Execute(null);
        }

        [Then(@"I see the following songs")]
        public void ThenISeeTheFollowingSongs(Table table)
        {
            var vm = context.Get<FoldersViewModel>();
            vm.FilteredSongs.Count().Should().Be(table.RowCount);
        }

        [Given(@"I load all the songs")]
        public async Task GivenILoadAllTheSongs()
        {
            var vm = context.Get<FoldersViewModel>();
            await vm.Loading();
        }

    }
}
