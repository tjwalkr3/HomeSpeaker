using HomeSpeaker.Maui.ViewModels;
namespace HomeSpeaker.Maui.Views;

public partial class StartPage : ContentPage
{
	public StartPage(StartPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}