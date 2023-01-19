namespace HomeSpeaker.Maui.Views;

public partial class ListDetailPage : ContentPage
{
    ListDetailViewModel ViewModel;

    public ListDetailPage(ListDetailViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = ViewModel = viewModel;
    }
}
