using AzureSignalRService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace AzureSignalRService
{
    public class SignalRService : ISignalRService
    {
        private readonly string signalREndPoint;
        private readonly string negotiateUri;
        private readonly string postUri;
        private readonly string messageName = "newMessage";
        private readonly HttpClient client;
        public event MessageReceivedHandler NewMessageReceived;
        public event ConnectionHandler Connected;
        public event ConnectionHandler ConnectionFailed;
        public bool IsConnected { get; private set; }
        public bool IsBusy { get; private set; }

        public SignalRService(IConfiguration configuration)
        {
            signalREndPoint = configuration["SignalREndpoint"];
            negotiateUri = configuration["NegotiateUri"];
            postUri = configuration["PostUri"];

            client = new HttpClient();
        }
        public SignalRService(string signalREndPoint, string negotiateUri, string postUri)
        {
            this.signalREndPoint = signalREndPoint;
            this.negotiateUri = negotiateUri;
            this.postUri = postUri;

            client = new HttpClient();
        }
        public async Task SendMessageAsync(string username, string message)
        {
            IsBusy = true;

            var newMessage = new Message
            {
                Name = username,
                Text = message
            };

            var json = JsonConvert.SerializeObject(newMessage);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(new Uri(new Uri(signalREndPoint),postUri) , content);

            IsBusy = false;
        }

        public async Task ConnectAsync()
        {
            try
            {
                IsBusy = true;

                string negotiateJson = await client.GetStringAsync(new Uri(new Uri(signalREndPoint), negotiateUri ));
                NegotiateInfo negotiate = JsonConvert.DeserializeObject<NegotiateInfo>(negotiateJson);

                HubConnection connection = new HubConnectionBuilder()
                    .AddNewtonsoftJsonProtocol()
                    .WithUrl(negotiate.Url, options =>
                    {
                        options.AccessTokenProvider = async () => await Task.FromResult(negotiate.AccessToken);
                    })
                    .Build();

                connection.Closed += Connection_Closed;
                connection.On<JObject>(messageName, AddNewMessage);
                await connection.StartAsync();

                IsConnected = true;
                IsBusy = false;

                Connected?.Invoke(this, true, "Connection successful.");
            }
            catch (Exception ex)
            {
                ConnectionFailed?.Invoke(this, false, ex.Message);
                IsConnected = false;
                IsBusy = false;
            }
        }

        Task Connection_Closed(Exception arg)
        {
            ConnectionFailed?.Invoke(this, false, arg.Message);
            IsConnected = false;
            IsBusy = false;
            return Task.CompletedTask;
        }

        void AddNewMessage(JObject message)
        {
            Message messageModel = new Message
            {
                Name = message.GetValue("name").ToString(),
                Text = message.GetValue("text").ToString(),
                TimeReceived = DateTime.Now
            };

            NewMessageReceived?.Invoke(this, messageModel);
        }

    }
    public delegate void MessageReceivedHandler(object sender, Message message);
    public delegate void ConnectionHandler(object sender, bool successful, string message);
}
