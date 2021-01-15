using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Mobile.ViewModels
{
    public class SongViewModel
    {
        public SongViewModel()
        {

        }
        public int SongId { get; set; }
        public string Name { get; set; }
        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
                Folder = System.IO.Path.GetDirectoryName(path);
            }
        }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Folder { get; set; }

        private Command playSong;
        public Command PlaySong => playSong ??= (new Command(() =>
        {
            var client = DependencyService.Get<HomeSpeakerClient>();
            client.PlaySong(new Server.gRPC.PlaySongRequest { SongId = SongId });
        }));

        private Command enqueueSong;
        public Command EnqueueSong => enqueueSong ??= new Command(() =>
        {
            var client = DependencyService.Get<HomeSpeakerClient>();
            client.EnqueueSong(new Server.gRPC.PlaySongRequest { SongId = SongId });
        });

        private Command starSong;
        public Command StarSong => starSong ??= new Command(async () =>
        {
            await App.Database.SaveStarredSongAsync(new Models.StarredSong { Path = Path });
        });

        private Command unStarSong;
        public Command UnStarSong => unStarSong ??= new Command(async () =>
        {
            await App.Database.DeleteStarredSong(new Models.StarredSong { Path = Path });
        });
    }

    public class SongGroup : List<SongViewModel>, INotifyPropertyChanged
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }

        public SongGroup(string name, List<SongViewModel> songs) : base(songs)
        {
            var di = new DirectoryInfo(name);
            FolderName = di.Name;
            FolderPath = di.Parent.FullName;
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;
            set { SetField(ref isExpanded, value); }
        }

        private Command playFolder;
        public Command PlayFolder => playFolder ??= new Command(() =>
        {
            var client = DependencyService.Get<HomeSpeakerClient>();
            client.PlayerControl(new Server.gRPC.PlayerControlRequest { Stop = true, ClearQueue = true });
            foreach(var s in this)
            {
                client.EnqueueSong(new Server.gRPC.PlaySongRequest { SongId = s.SongId });
            }
        });

        private Command enqueueFolder;
        public Command EnqueueFolder => enqueueFolder ??= new Command(() =>
        {
            var client = DependencyService.Get<HomeSpeakerClient>();
            foreach (var s in this)
            {
                client.EnqueueSong(new Server.gRPC.PlaySongRequest { SongId = s.SongId });
            }
        });

        private Command starFolder;
        public Command StarFolder => starFolder ??= new Command(async () =>
        {
            foreach(var s in this)
            {
                await App.Database.SaveStarredSongAsync(new Models.StarredSong { Path = s.Path });
            }
            this.Clear();
        });

        private Command unStarFolder;
        public Command UnStarFolder => unStarFolder ??= new Command(async () =>
        {
            foreach (var s in this)
            {
                await App.Database.DeleteStarredSong(s.Path);
            }
            this.Clear();
        });

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
