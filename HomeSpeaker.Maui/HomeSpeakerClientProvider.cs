using Grpc.Net.Client;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui;

public class HomeSpeakerClientProvider
{
    public HomeSpeakerClientProvider()
    {
        ReloadClientFromPreferences();
    }

    public void ReloadClientFromPreferences()
    {
        if (Preferences.ContainsKey(Constants.ServerAddress) is false)
        {
            Preferences.Set(Constants.ServerAddress, "http://192.168.1.110");
        }
        var baseUri = new Uri(Preferences.Get(Constants.ServerAddress, "http://192.168.1.110"));
        var channel = GrpcChannel.ForAddress(baseUri);
        Client = new HomeSpeakerClient(channel);
    }

    private object lockObject = new();
    private HomeSpeakerClient client;

    public HomeSpeakerClient Client
    {
        get
        {
            lock (lockObject)
            {
                return client;
            }
        }
        private set
        {
            lock (lockObject)
            {
                client = value;
            }
        }
    }
}
