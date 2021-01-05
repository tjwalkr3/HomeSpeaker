using HomeSpeaker.Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace HomeSpeaker.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}