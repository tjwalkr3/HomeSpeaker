namespace HomeSpeaker.Maui.Views;

public partial class StatusPage : ContentPage
{
	public StatusPage(StatusViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}