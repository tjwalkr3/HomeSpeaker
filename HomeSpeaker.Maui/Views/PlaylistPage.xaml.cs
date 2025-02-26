namespace HomeSpeaker.Maui.Views;
using HomeSpeaker.Maui.ViewModels;

public partial class PlaylistPage : ContentPage
{
    public PlaylistPage(PlaylistPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}