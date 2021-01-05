using Grpc.Core;
using HomeSpeaker.Mobile.Services;
using HomeSpeaker.Mobile.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            Device.SetFlags(new[] { "Expander_Experimental" });

            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.RegisterSingleton(new HomeSpeakerClient(new Channel("192.168.1.20:8080", ChannelCredentials.Insecure)));
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
