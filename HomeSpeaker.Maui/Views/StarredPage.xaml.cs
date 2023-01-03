namespace HomeSpeaker.Maui.Views;

public partial class StarredPage : ContentPage
{
    private readonly StarredViewModel starredViewModel;

    public StarredPage(StarredViewModel starredViewModel)
    {
        InitializeComponent();
        this.BindingContext = this.starredViewModel = starredViewModel;
    }
}