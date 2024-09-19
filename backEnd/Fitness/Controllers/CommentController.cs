using Fitness.BLL;
using Fitness.BLL.Interfaces;
using Fitness.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentBLL _commentBLL;

        public CommentController()
        {
            _commentBLL = new CommentBLL();
        }

        [HttpPost]
        public ActionResult<string> PublishComment( string token, [FromBody] Comment comment)
        {
            return _commentBLL.PublishComment(token, comment);
        }

        [HttpPost]
        public ActionResult<string> ReplyComment(string token, [FromBody] Comment comment)
        {
            return _commentBLL.ReplyComment(token, comment);
        }

        [HttpGet]
        public ActionResult<List<Comment>> GetCommentByPostID(string token, int postID)
        {
            var comments = _commentBLL.GetCommentByPostID(token, postID);
            if (comments != null && comments.Count > 0)
            {
                return Ok(comments);
            }
            return NotFound("未找到相关评论");
        }

        [HttpGet]
        public ActionResult<Comment> GetCommentByCommentID(string token, int commentID)
        {
            var comment = _commentBLL.GetCommentByCommentID(token, commentID);
            if (comment != null)
            {
                return Ok(comment);
            }
            return NotFound("未找到该评论");
        }

        [HttpDelete]
        public ActionResult<string> DeleteComment(string token, int commentID ,int postID)
        {
            var result = _commentBLL.DeleteComment(token, commentID,postID);
            Console.WriteLine(result);
            if (result == "评论删除成功")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public ActionResult<string> LikeComment(string token, int commentID)
        {
            var result = _commentBLL.LikeComment(token, commentID);
            if (result == "点赞成功")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public ActionResult<string> CancleLikeComment(string token, int commentID)
        {
            var result = _commentBLL.CancleLikeComment(token, commentID);
            if (result == "取消点赞成功")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
