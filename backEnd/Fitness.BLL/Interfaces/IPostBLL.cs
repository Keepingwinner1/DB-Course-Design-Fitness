using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IPostBLL
    {
        public string Post(string token, Post post);
        public List<Post> GetPostByUserID(string token,int userID);
        public PostInfo GetPostByPostID(string token, int postID);
        public string GetAllPost(string token);
        public string Delete(string token, int postId, int postOwnerID);
        public string LikePost(string token, int postId,int postOwnerID);
        public string CancleLike(string token, int postId, int postOwnerID);
        public string Report(string token, int postId);
        public string CancleReport(string token, int postId);
        public string Forward(string token, int postId);
        public List<Post> Search(string token, string query, string category = null, string dateRange = null, string sortBy = "postTime");
        // 评论区AI
        // 健身教练AI
        public MessageRes FitCoachComment(string postTitle, string postContent);
        // 营养顾问AI
        public MessageRes NutriExpertComment(string postTitle,string postContent);
        // 激励导师AI
        public MessageRes MotivatorComment(string postTitle, string postContent);

    }
}
