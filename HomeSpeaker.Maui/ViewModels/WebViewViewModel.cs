namespace HomeSpeaker.Maui.ViewModels;

public partial class WebViewViewModel : BaseViewModel
{
    [ObservableProperty]
    public string source;

    public WebViewViewModel()
    {
        Source = "https://github.com/snow-jallen/HomeSpeaker";
    }

    [RelayCommand]
    private async void WebViewNavigated(WebNavigatedEventArgs e)
    {
        if (e.Result != WebNavigationResult.Success)
        {
            await Shell.Current.DisplayAlert("Navigation failed", e.Result.ToString(), "OK");
        }
    }

    [RelayCommand]
    private void NavigateBack(WebView webView)
    {
        if (webView.CanGoBack)
        {
            webView.GoBack();
        }
    }

    [RelayCommand]
    private void NavigateForward(WebView webView)
    {
        if (webView.CanGoForward)
        {
            webView.GoForward();
        }
    }

    [RelayCommand]
    private void RefreshPage(WebView webView)
    {
        webView.Reload();
    }

    [RelayCommand]
    private async void OpenInBrowser()
    {
        await Launcher.OpenAsync(Source);
    }
}
