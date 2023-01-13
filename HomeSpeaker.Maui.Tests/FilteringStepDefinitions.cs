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
            var vm = new FoldersViewModel(staredSongMock.Object, playerServiceMock.Object);

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
        public void WhenISetTheFilterTextTo(int p0)
        {
            throw new PendingStepException();
        }

        [When(@"click the Filter button")]
        public void WhenClickTheFilterButton()
        {
            throw new PendingStepException();
        }

        [Then(@"I see the following songs")]
        public void ThenISeeTheFollowingSongs(Table table)
        {
            throw new PendingStepException();
        }
    }
}
