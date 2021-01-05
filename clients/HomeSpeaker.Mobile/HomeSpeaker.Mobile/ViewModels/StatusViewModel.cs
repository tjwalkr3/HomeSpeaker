using HomeSpeaker.Mobile.Models;
using HomeSpeaker.Mobile.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeSpeaker.Mobile.ViewModels
{
    public class StatusViewModel : BaseViewModel
    {
        public StatusViewModel()
        {
            Title = "Status";
        }

        private Command refreshStatusCommand;
        public Command RefreshStatusCommand => refreshStatusCommand ??= new Command(() =>
        {
            IsBusy = true;
            try
            {

            }
            catch(Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        });

        public void OnAppearing()
        {
            IsBusy = true;
        }

        //async void OnItemSelected(Item item)
        //{
        //    if (item == null)
        //        return;

        //    // This will push the ItemDetailPage onto the navigation stack
        //    await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        //}
    }
}