using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class Comment
    {
        public int commentID { get; set; }

        public int userID { get; set; }

        public string userName {get; set; }

        public int postID { get; set; }

        // 父评论ID（如果是根评论则为-1）
        public int parentCommentID { get; set; }

        public DateTime commentTime { get; set; }

        public int likesCount { get; set; }

        public string Content { get; set; }
    }

}
