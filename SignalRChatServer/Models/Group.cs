using System.Collections.Generic;

namespace SignalRChatServer.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<Client> Clients { get; set; } =  new List<Client>();    
    }
}
