using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using HomeSpeaker.Shared;

namespace HomeSpeaker.WebAssembly.Services
{
    public class HomeSpeakerService
    {
        private HomeSpeaker.Shared.HomeSpeaker.HomeSpeakerClient client;
        private List<SongMessage> songs = new();

        public HomeSpeakerService(IConfiguration config)
        {
            var channel = GrpcChannel.ForAddress(config["ServerAddress"], new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });

            client = new HomeSpeaker.Shared.HomeSpeaker.HomeSpeakerClient(channel);
        }

        public async Task<GetStatusReply> GetStatusAsync()
        {
            return await client.GetPlayerStatusAsync(new GetStatusRequest());
        }

        public IEnumerable<SongMessage> Songs => songs;

        public async Task StartPlayingAsync()
        {
            if (!songs.Any())
            {
                await GetSongsAsync();
            }
            await client.PlayFolderAsync(new PlayFolderRequest { FolderPath = songs.First().Path });
        }

        private async Task GetSongsAsync()
        {
            var streamingCall = client.GetSongs(new GetSongsRequest());
            try
            {
                await foreach (var getSongsReply in streamingCall.ResponseStream.ReadAllAsync())
                {
                    songs.AddRange(getSongsReply.Songs);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }
    }
}
