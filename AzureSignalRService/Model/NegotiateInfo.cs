using System;

namespace AzureSignalRService.Model
{
    public class NegotiateInfo
    {
        public Uri Url
        {
            get;
            set;
        }

        public string AccessToken
        {
            get;
            set;
        }
    }
}