using HomeSpeaker.Maui.ViewModels;

namespace HomeSpeaker.Maui.Views;

public partial class ChangeMetadata : ContentPage
{
	public ChangeMetadata(ChangeMetadataViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}