using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRChatServer.Data;
using SignalRChatServer.Models;
using Group = SignalRChatServer.Models.Group;

namespace SignalRChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetNickName (string nickname)
        {
            Client client = new Client
            {
                ConnectionId = Context.ConnectionId,
                NickName = nickname
            };
            ClientSource.Clients.Add(client);

            await Clients.Others.SendAsync("clientJoined", nickname);
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }

        public async Task SendMessageAsync(string message, string clientName)
        {
            clientName = clientName.Trim();
            Client senderClient = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (clientName =="Tümü")
            {
                await Clients.Others.SendAsync("receiveMessage", message, senderClient.NickName);
            }
            else
            { 
                Client client =  ClientSource.Clients.FirstOrDefault(x => x.NickName == clientName);
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.NickName);
                    
            }
                        
        }

        public async Task AddGroup(string groupName)
        {
             await   Groups.AddToGroupAsync(Context.ConnectionId, groupName);

             Group group = new Group {GroupName = groupName, };
             group.Clients.Add(ClientSource.Clients.FirstOrDefault(c =>c.ConnectionId == Context.ConnectionId));

             GroupSource.Groups.Add(group);

             await Clients.All.SendAsync("groups", GroupSource.Groups);

        }

        public async Task AddClientToGroup(IEnumerable<string> groupNames)
        {
            Client client = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            foreach (var group in groupNames)
            {
                Group _group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == Context.ConnectionId);

                var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);
                _group.Clients.Add(client);
                if (!result)
                {
                    _group.Clients.Add(client);
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                }
            }
        }

        public async Task GetClientToGroup(string groupName)
        {
         

            Group group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == groupName);

           await Clients.Caller.SendAsync("clients",groupName == "-1" ? ClientSource.Clients : group.Clients);
        }

        public async Task SendMessageToGroupAsync(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("receiveMessage", message,
                ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId).NickName);
        }
    }
}
 