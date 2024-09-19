using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.BLL;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;

namespace Fitness.Models
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        //int messageID, int senderID, int receiverID,string messageType, string Content, string sendTime
        public async Task Send(int messageID, int senderID, int receiverID, string messageType, string Content, string sendTime)
        {
            try
            {
                Console.WriteLine("11111");
                await Clients.All.SendAsync("ReceiveMessage", messageID, senderID, receiverID, messageType, Content, sendTime);
               // await Clients.All.SendAsync("ReceiveMessage", messageID, senderID,receiverID,messageType,Content,sendTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to friend");
                throw; // 抛出异常以确保客户端知道有问题
            }
            /*  MessageBLL messageBLL = new MessageBLL();
                messageBLL.InsertMessage(message);*/
        }

    }
}
