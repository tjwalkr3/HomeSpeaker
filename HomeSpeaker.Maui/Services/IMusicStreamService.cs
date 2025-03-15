using HomeSpeaker.Maui.Models;

namespace HomeSpeaker.Maui.Services
{
    public interface IMusicStreamService
    {
        Task<List<StreamModel>> Search(string query);
    }
}