using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class Message
    {
        // 消息ID
        public int messageID { get; set; }

        // 发送者ID
        public int senderID { get; set; }

        // 接收者ID
        public int receiverID { get; set; }

        // 消息类型（如：通知、私信等）
        public string messageType { get; set; }

        // 消息内容
        public string Content { get; set; }

        // 发送时间
        public DateTime sendTime { get; set; }

        public int isRead { get; set; }

    }
}
