using Grpc.Core;
using HomeSpeaker.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Threading;

namespace HomeSpeaker.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public List<Song> Songs { get; private set; }
        public IEnumerable<Song> Queue { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            Songs = new List<Song>();
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");

            var channel = new Channel("192.168.1.20:8080", ChannelCredentials.Insecure);
            HomeSpeaker.Server.gRPC.HomeSpeaker.HomeSpeakerClient client = new Server.gRPC.HomeSpeaker.HomeSpeakerClient(channel);
            var getSongsReply = client.GetSongs(new Server.gRPC.GetSongsRequest { });
            await foreach (var reply in getSongsReply.ResponseStream.ReadAllAsync())
            {
                Songs.AddRange(reply.Songs.Select(s => s.ToSong()));
            }

            var getQueueReply = client.GetPlayQueue(new Server.gRPC.GetSongsRequest { });
            await foreach (var reply in getQueueReply.ResponseStream.ReadAllAsync())
            {
                Queue = from songMessage in reply.Songs
                        select songMessage.ToSong();
            }
        }
    }

    public class Song
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
    }

    public static class Extensions
    {

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
        public async static IAsyncEnumerable<T> ReadAllAsync<T>(this IAsyncStreamReader<T> streamReader, CancellationToken cancellationToken = default)
        {
            if (streamReader == null)
            {
                throw new System.ArgumentNullException(nameof(streamReader));
            }

            while (await streamReader.MoveNext(cancellationToken))
            {
                yield return streamReader.Current;
            }
        }
    }
}
