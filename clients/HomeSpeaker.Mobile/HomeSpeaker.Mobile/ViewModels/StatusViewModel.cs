using HomeSpeaker.Mobile.Models;
using HomeSpeaker.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Mobile.ViewModels
{
    public class StatusViewModel : BaseViewModel
    {
        public StatusViewModel()
        {
            Title = "Status";

            client = DependencyService.Get<HomeSpeakerClient>();
        }

        public string Name { get; set; }
        public string Folder { get; set; }

        private string nowPlayingTitle;
        public string NowPlayingTitle
        {
            get => nowPlayingTitle;
            set { SetProperty(ref nowPlayingTitle, value); }
        }

        private TimeSpan elapsed;
        public TimeSpan Elapsed
        {
            get => elapsed;
            set { SetProperty(ref elapsed, value); }
        }

        private TimeSpan remaining;
        public TimeSpan Remaining
        {
            get => remaining;
            set { SetProperty(ref remaining, value); }
        }

        private double percentComplete;
        public double PercentComplete
        {
            get => percentComplete;
            set { SetProperty(ref percentComplete, value); }
        }
        private IEnumerable<SongViewModel> queue;
        public IEnumerable<SongViewModel> Queue
        {
            get => queue;
            set => SetProperty(ref queue, value);
        }
        private Command stop;
        public Command Stop => stop ??= new Command(() =>
        {
            client.PlayerControl(new Server.gRPC.PlayerControlRequest { Stop = true });
            RefreshStatusCommand.Execute(this);
        });
        private Command play;
        public Command Play => play ??= new Command(() =>
        {
            client.PlayerControl(new Server.gRPC.PlayerControlRequest { Play = true });
            RefreshStatusCommand.Execute(this);
        });
        private Command forward;
        public Command Forward => forward ??= new Command(() =>
        {
            client.PlayerControl(new Server.gRPC.PlayerControlRequest { SkipToNext = true });
            RefreshStatusCommand.Execute(this);
        });
        private Command clear;
        public Command Clear => clear ??= new Command(() =>
        {
            client.PlayerControl(new Server.gRPC.PlayerControlRequest { ClearQueue = true });
            RefreshStatusCommand.Execute(this);
        });

        private HomeSpeakerClient client;

        private Command refreshStatusCommand;
        public Command RefreshStatusCommand => refreshStatusCommand ??= new Command(async () =>
        {
            IsBusy = true;
            try
            {

                var getQueueReply = client.GetPlayQueue(new Server.gRPC.GetSongsRequest { });
                await foreach (var reply in getQueueReply.ResponseStream.ReadAllAsync())
                {
                    Queue = from songMessage in reply.Songs
                            select songMessage.ToSongViewModel();
                }

                var statusReply = client.GetPlayerStatus(new Server.gRPC.GetStatusRequest { });
                NowPlayingTitle = statusReply.CurrentSong.Name;
                Elapsed = statusReply.Elapsed.ToTimeSpan();
                Remaining = statusReply.Remaining.ToTimeSpan();
                PercentComplete = statusReply.PercentComplete;
            }
            catch(Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        });

        public void OnAppearing()
        {
            IsBusy = true;
        }

        //async void OnItemSelected(Item item)
        //{
        //    if (item == null)
        //        return;

        //    // This will push the ItemDetailPage onto the navigation stack
        //    await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        //}
    }
}