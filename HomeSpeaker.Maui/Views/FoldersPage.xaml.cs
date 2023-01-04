namespace HomeSpeaker.Maui.Views;

public partial class FoldersPage : ContentPage
{
	public FoldersPage(FoldersViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}