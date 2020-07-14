using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GroupUp.Models;
using Microsoft.AspNet.SignalR;

namespace GroupUp
{
    public class GroupUpHub : Hub
    {
        public readonly ApplicationDbContext _context;
        public GroupUpHub()
        {
            _context = new ApplicationDbContext();
        }
        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        public void Send(string name, string message, string roomName)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.Group(roomName).addNewMessageToPage(name, message);
        }
    }
}