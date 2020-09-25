using AzureSignalRService;
using AzureSignalRService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SignalRChatClient.Mobile
{
    public partial class MainPage : ContentPage
    {
        private ISignalRService signalRService;

        ObservableCollection<string> ChatList = new ObservableCollection<string>();

        public MainPage()
        {
            signalRService = DependencyService.Get<ISignalRService>();
            signalRService.ConnectAsync();
            signalRService.NewMessageReceived += SignalRService_NewMessageReceived;
            InitializeComponent();
            this.lstView.ItemsSource = ChatList;
        }


        private void SignalRService_NewMessageReceived(object sender, Message message)
        {
            //Device.BeginInvokeOnMainThread(()=> {
            ChatList.Add($"{message.Name} ({message.TimeReceived}) - {message.Text}");

            //scroll to last
            var last = lstView.ItemsSource.Cast<object>().LastOrDefault();
            lstView.ScrollTo(last, ScrollToPosition.MakeVisible, true);
            //});

        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            await signalRService.SendMessageAsync("Xamarin User", txtEntry.Text);
            txtEntry.Text = string.Empty;
        }
    }
}
