using Grpc.Core;
using HomeSpeaker.Mobile.Services;
using HomeSpeaker.Mobile.Views;
using System;
using System.IO;
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
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "starredsongs.db3"));
                }
                return database;
            }
        }

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
