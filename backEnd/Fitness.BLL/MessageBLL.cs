using Fitness.DAL;
using Fitness.Models;
using Microsoft.AspNetCore.SignalR;
using Fitness.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL
{
    public class MessageBLL:IMessageBLL
    {
        public bool InsertMessage(Message message)
        {
            bool result = MessageDAL.Insert(message);
            return result;
        }

        public List<Message> GetChatHistory(int userId)
        {

            List<Message> messages = MessageDAL.GetMessageByUserID(userId);

            return messages;
        }

    }
}
