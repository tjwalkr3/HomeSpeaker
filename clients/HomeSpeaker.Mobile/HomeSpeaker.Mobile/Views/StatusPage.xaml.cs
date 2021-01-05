using HomeSpeaker.Mobile.Models;
using HomeSpeaker.Mobile.ViewModels;
using HomeSpeaker.Mobile.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeSpeaker.Mobile.Views
{
    public partial class StatusPage : ContentPage
    {
        StatusViewModel _viewModel;

        public StatusPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new StatusViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}