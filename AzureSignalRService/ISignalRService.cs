using AzureSignalRService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSignalRService
{
    public interface ISignalRService
    {
        public event MessageReceivedHandler NewMessageReceived;
        public event ConnectionHandler Connected;
        public event ConnectionHandler ConnectionFailed;

        Task SendMessageAsync(string username, string message);
        Task ConnectAsync();

        bool IsConnected { get; }
        bool IsBusy { get; }

    }
}
