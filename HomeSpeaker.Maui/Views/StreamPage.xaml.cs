namespace HomeSpeaker.Maui.Views;
using HomeSpeaker.Maui.ViewModels;

public partial class StreamPage : ContentPage
{
	public StreamPage(StreamPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}