public class ShellNavigationService : INavigationService
{
    public Task GoToAsync(string route)
    {
        return Shell.Current.GoToAsync(route);
    }
}