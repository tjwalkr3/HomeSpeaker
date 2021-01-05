using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Mobile.ViewModels
{
    public class FoldersViewModel : BaseViewModel
    {
        public FoldersViewModel()
        {
            Songs = new ObservableCollection<SongGroup>();
            this.client = DependencyService.Get<HomeSpeakerClient>();
            init();
        }

        private readonly HomeSpeakerClient client;
        public Command LoginCommand { get; }
        public ObservableCollection<SongGroup> Songs { get; private set; }
        private IEnumerable<SongViewModel> queue;
        public IEnumerable<SongViewModel> Queue
        {
            get => queue;
            set => SetProperty(ref queue, value);
        }
        private string status;
        public string Status
        {
            get => status;
            set
            {
                SetProperty(ref status, value);
                OnPropertyChanged(nameof(StatusIsVisible));
            }
        }
        public bool StatusIsVisible => String.IsNullOrWhiteSpace(Status) is false;

        private async void init()
        {
            Status = "getting song info...";
            var groups = new Dictionary<string, List<SongViewModel>>();
            var getSongsReply = client.GetSongs(new Server.gRPC.GetSongsRequest { });
            await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
            {
                foreach (var s in reply.Songs)
                {
                    var song = s.ToSongViewModel();
                    if (groups.ContainsKey(song.Folder) is false)
                        groups[song.Folder] = new List<SongViewModel>();
                    groups[song.Folder].Add(song);
                }
            }

            foreach (var group in groups.OrderBy(g => g.Key))
            {
                Songs.Add(new SongGroup(group.Key, group.Value.OrderBy(s=>s.Path).ToList()));
            }

            var getQueueReply = client.GetPlayQueue(new Server.gRPC.GetSongsRequest { });
            await foreach (var reply in getQueueReply.ResponseStream.ReadAllAsync())
            {
                Queue = from songMessage in reply.Songs
                        select songMessage.ToSongViewModel();
            }

            Status = null;

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");

        }
    }
}