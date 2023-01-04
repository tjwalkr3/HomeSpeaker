namespace HomeSpeaker.Maui.Views;

public partial class StreamPage : ContentPage
{
	public StreamPage(StreamViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}