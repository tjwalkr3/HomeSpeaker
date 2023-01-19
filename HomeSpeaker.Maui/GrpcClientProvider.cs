using Grpc.Net.Client;
using static HomeSpeaker.Shared.HomeSpeaker;

namespace HomeSpeaker.Maui;

public class GrpcClientProvider
{
    public GrpcClientProvider()
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
        lock (lockObject)
        {
            Client = new HomeSpeakerClient(channel);
        }
    }

    private object lockObject = new();

    public HomeSpeakerClient Client { get; private set; }
}
