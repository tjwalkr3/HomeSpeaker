using Grpc.Core;
using HomeSpeaker.Mobile.Services;
using HomeSpeaker.Mobile.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Mobile
{
    public partial class App : Application
    {
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "homespeaker_starredsongs.db3"));
                }
                return database;
            }
        }

        public App()
        {
            Device.SetFlags(new[] { "Expander_Experimental" });

            InitializeComponent();

            var endpointAddress = "192.168.1.133:8080";
            if(isNotAvailable(endpointAddress) || Debugger.IsAttached)
            {
                //endpointAddress = "192.168.1.140:5000";
                //endpointAddress = "144.17.10.32:5000";
            }

            DependencyService.Register<MockDataStore>();
            DependencyService.RegisterSingleton(new HomeSpeakerClient(new Channel(endpointAddress, ChannelCredentials.Insecure)));
            MainPage = new AppShell();

            var lastPage = Preferences.Get("lastPage", "//StreamPage");
            Shell.Current.GoToAsync(lastPage);
        }

        private bool isNotAvailable(string endpointAddress)
        {
            try
            {
                var parts = endpointAddress.Split(':');
                using var tcpClient = new TcpClient(parts[0], int.Parse(parts[1]));
                return true;
            }
            catch
            {
                return false;
            }
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
