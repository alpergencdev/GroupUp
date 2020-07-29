using System;
using System.Linq;
using System.Threading.Tasks;
using GroupUp.Models;
using Microsoft.AspNet.SignalR;

namespace GroupUp
{
    // This class is a SignalR hub to coordinate group chats, and their chatlogs.
    public class GroupUpHub : Hub
    {
        private readonly ApplicationDbContext _context;
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
            try
            {
                // ReSharper disable once InvertIf
                if (message != "")
                {
                    int roomNo = Int32.Parse(roomName);
                    // first, add the message to the ChatLog of the group.
                    _context.Groups.Single(g => g.GroupId == roomNo).ChatLog += $"<strong>{name}: </strong>{message}\n";
                    _context.SaveChanges();
                    // then, send it to the clients of the group.
                    Clients.Group(roomName).addNewMessageToPage(name, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}