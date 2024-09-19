using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IMessageBLL
    {
        public bool InsertMessage(Message message);
        public List<Message> GetChatHistory(int userId);
    }
}
