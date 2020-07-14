using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace GroupUp
{
    public class GroupUpHub : Hub
    {
        public GroupUpHub()
        {

        }
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}