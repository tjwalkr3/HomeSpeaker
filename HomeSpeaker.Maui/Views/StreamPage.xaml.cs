using HomeSpeaker.Maui.ViewModels;

namespace HomeSpeaker.Maui.Views;

public partial class StreamPage : ContentPage
{
	public StreamPage(StreamPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}