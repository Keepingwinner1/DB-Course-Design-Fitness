using Fitness.BLL.Core;
using Fitness.BLL.Interfaces;
using Fitness.DAL;
using Fitness.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fitness.BLL
{
    public class CommentBLL: ICommentBLL
    {
        //PublishComment   ReplyComment
        //GetCommentByPostID  GetCommentByCommentID
        //DeleteComment
        //likeComment

        private readonly JWTHelper _jwtHelper = new();
        private readonly UserAchievementBLL _userAchievement = new UserAchievementBLL();
        private readonly PostBLL postbll = new PostBLL();

        public string PublishComment(string token, Comment comment)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int senderId = tokenRes.userID;
            //comment.userID = senderId;
            comment.commentTime = DateTime.Now;
            comment.likesCount = 0;
            comment.parentCommentID = -1;
            int st;
            //comment.userName = UserDAL.GetUserByUserID(tokenRes.userID,out st).userName;
            int result = CommentDAL.Insert(comment);
            //帖子评论量+1
            CommentDAL.CommentsCountAddOne(comment.postID);
            //更新成就
            int achieveUserID = postbll.GetPostByPostID(token, comment.postID).userID;
            _userAchievement.UpdateBeComment(achieveUserID, 1);
            //调用通知接口
            Message message = new Message() {
                messageID = -1,
                senderID = senderId,
                receiverID = achieveUserID,
                messageType = "评论",
                Content = $"{comment.Content}",
                sendTime = DateTime.Now,
                isRead=0
            };
            MessageDAL.Insert(message);
            return JsonConvert.SerializeObject(new
            {
                commentID = result,
                message = "发布评论成功"
            });
        }

        public string ReplyComment(string token, Comment replyComment)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int senderId = tokenRes.userID;
            replyComment.userID = senderId;
            replyComment.commentTime = DateTime.Now;
            replyComment.likesCount = 0;
            int st;
            replyComment.userName = UserDAL.GetUserByUserID(tokenRes.userID, out st).userName;
            int  result =CommentDAL.Insert(replyComment);
            //帖子评论量+1
            CommentDAL.CommentsCountAddOne(replyComment.postID);
            //更新成就
            int achieveUserID = postbll.GetPostByPostID(token, replyComment.postID).userID;
            _userAchievement.UpdateBeComment(achieveUserID, 1);
            //调用通知接口
            Message message = new Message()
            {
                messageID = -1,
                senderID = senderId,
                receiverID = achieveUserID,
                messageType = "回复",
                Content = $"{replyComment.Content}",
                sendTime = DateTime.Now,
                isRead = 0
            };
            MessageDAL.Insert(message);
            return JsonConvert.SerializeObject(new
            {
                commentID = result,
                message = "回复成功"
            });
        }

        public List<Comment> GetCommentByPostID(string token,int postID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            return CommentDAL.GetCommentByPostID(postID);
        }

        public List<Comment> GetCommentByCommentID(string token, int commentID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            return CommentDAL.GetCommentByCommentID(commentID);
        }

        public string DeleteComment(string token, int commentID, int postID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            bool result = false;
            PostBLL postBLL = new PostBLL();
            int userid = tokenRes.userID;
            int PostUserID = postBLL.GetPostByPostID(token, postID).userID;
            int commentOwnerID = CommentDAL.GetByCommentID(commentID).userID;
            bool isCommentOwner = false;
            bool isPostOwner = false;
            if (PostUserID == userid) {
                isPostOwner = true;
            }
            if (commentOwnerID == userid) { 
                isCommentOwner = true;
            }
            if (tokenRes.Role == "admin" || isPostOwner ||isCommentOwner)
            {
                CommentDAL.CommentsCountMinusOne(postID);
                result = CommentDAL.DeleteComment(commentID);
            }
            else
                return "身份权限不符";
            //更新成就
            _userAchievement.UpdateBeComment(PostUserID, -1);
            return result ?"评论删除成功" : "评论删除失败";
        }

        public string LikeComment(string token, int commentID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            bool result = CommentDAL.LikeCountsAddOne(commentID);
            var comment = CommentDAL.GetByCommentID(commentID);
            //调用通知接口
            Message message = new Message()
            {
                messageID = -1,
                senderID = tokenRes.userID,
                receiverID = comment.userID,
                messageType = "点赞评论",
                Content = $"{comment.Content}",
                sendTime = DateTime.Now,
                isRead = 0
            };
            MessageDAL.Insert(message);
            return result ? "点赞成功" : "点赞失败";
        }

        public string CancleLikeComment(string token, int commentID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            bool result = CommentDAL.LikeCountsMinusOne(commentID);
            return result ? "取消点赞成功" : "取消点赞失败";
        }

    }
}
