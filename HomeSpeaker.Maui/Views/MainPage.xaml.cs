namespace HomeSpeaker.Maui.Views;
using HomeSpeaker.Maui.ViewModels;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
