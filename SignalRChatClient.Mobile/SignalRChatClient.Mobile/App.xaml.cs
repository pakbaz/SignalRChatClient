using AzureSignalRService;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SignalRChatClient.Mobile
{
    public partial class App : Application
    {  
        private readonly string signalREndPoint = "";
        private readonly string negotiateUri = "/api/negotiate";
        private readonly string postUri = "/api/talk";
        public App()
        {
            InitializeComponent();

            SignalRService signalRService = new SignalRService(signalREndPoint, negotiateUri, postUri);
            DependencyService.RegisterSingleton<ISignalRService>(signalRService);
            
            MainPage = new MainPage();
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
