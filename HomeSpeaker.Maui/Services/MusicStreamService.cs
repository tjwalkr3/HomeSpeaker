using System.Net.NetworkInformation;
using System.Net;
using System.Text.Json;
using HomeSpeaker.Maui.Models;
namespace HomeSpeaker.Maui.Services;

public class MusicStreamService : IMusicStreamService
{
    private readonly HttpClient _httpClient;

    public MusicStreamService()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"https://{GetRadioBrowserApiUrl()}")
        };
    }

    private static string GetRadioBrowserApiUrl()
    {
        // Get fastest ip of dns
        string baseUrl = @"all.api.radio-browser.info";
        var ips = Dns.GetHostAddresses(baseUrl);
        long lastRoundTripTime = long.MaxValue;
        string searchUrl = @"de1.api.radio-browser.info"; // Fallback
        foreach (IPAddress ipAddress in ips)
        {
            var reply = new Ping().Send(ipAddress);
            if (reply != null &&
                reply.RoundtripTime < lastRoundTripTime)
            {
                lastRoundTripTime = reply.RoundtripTime;
                searchUrl = ipAddress.ToString();
            }
        }

        // Get clean name
        IPHostEntry hostEntry = Dns.GetHostEntry(searchUrl);
        if (!string.IsNullOrEmpty(hostEntry.HostName))
        {
            searchUrl = hostEntry.HostName;
        }

        return searchUrl;
    }

    public async Task<List<StreamModel>> Search(string query)
    {
        List<StreamModel> streams = [];
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"/json/stations/byname/{query}?order=clickcount&limit=10");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                streams = JsonSerializer.Deserialize<List<StreamModel>>(result) ?? [];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return streams;
    }
}
