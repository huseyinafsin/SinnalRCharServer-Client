using System.Collections.Generic;
using SignalRChatServer.Models;

namespace SignalRChatServer.Data
{
    public static class ClientSource
    {
        public static List<Client> Clients { get;} = new List<Client>();
    }
}
