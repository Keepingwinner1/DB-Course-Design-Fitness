using Fitness.BLL.Core;
using Fitness.BLL.Interfaces;
using Fitness.DAL;
using Fitness.DAL.Core;
using Fitness.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fitness.BLL
{
    public class PostBLL :IPostBLL
    {

        private readonly JWTHelper _jwtHelper = new();
        private readonly UserAchievementBLL _userAchievement = new UserAchievementBLL();
/*        private readonly PostBLL postbll = new PostBLL();*/
        public string Post(string token, Post post)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            post.userID = tokenRes.userID;
            post.postID = 0;
            post.likesCount = 0;
            post.forwardCount = 0;
            post.postTime = DateTime.Now;
            post.commentsCount = 0;
            //post.refrencePostID = -1;
            int st;
            post.userName = UserDAL.GetUserByUserID(tokenRes.userID,out st).userName;
            
            // 首先要判断是否已经为url
            if (post.imgUrl != "null" && !UrlHelper.IsUrl(post.imgUrl)) {

                long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond;
                string objectName = $"{post.userID}_{timestamp}.png";
                int base64Index = post.imgUrl.IndexOf("base64,") + 7;
                post.imgUrl = post.imgUrl.Substring(base64Index);
                OSSHelper.UploadBase64ImageToOss(post.imgUrl, objectName);
                post.imgUrl = OSSHelper.GetPublicObjectUrl(objectName);

            }

            int postId = PostDAL.Post(post);
            if (postId == 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    postID = postId,
                    message = "发帖失败"
                });
            }
            
            Publish publish = new Publish();
            publish.postID = postId;
            publish.userID = post.userID;
            publish.publishTime = post.postTime;

            PublishDAL.Post(publish);
            //更新成就
            _userAchievement.UpdatePostAchievement(tokenRes.userID, 1);
            //插入一条AI评论


            return JsonConvert.SerializeObject(new
            {
                postID = postId,
                message = "发帖成功"
            });
        }


        public PostInfo GetPostByPostID(string token, int postID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            try
            {
                var post = PostDAL.GetPostByPostID(postID);
                return post;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievements for userID: {postID}: {ex.Message}");
                throw;
            }
        }

        public List<Post> GetPostByUserID(string token,int userID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userId = tokenRes.userID;
            if (userID != -1)
                userId = userID;
            try
            {
                DataTable result = PostDAL.GetByUserID(userId);
                if (result == null || result.Rows.Count == 0)
                {
                    return null;
                }
                var posts = new List<Post>();
                foreach (DataRow row in result.Rows)
                {
                    posts.Add(new Post ()
                    {
                        postID = Convert.ToInt32(row["postID"]),
                        userID = Convert.ToInt32(row["userID"]),
                        postTitle = row["postTitle"].ToString(),
                        postContent = row["postContent"].ToString(),
                        postCategory = row["postCategory"].ToString(),
                        postTime = Convert.ToDateTime(row["postTime"]),
                        likesCount = Convert.ToInt32(row["likesCount"]),
                        forwardCount = Convert.ToInt32(row["forwardCount"]),
                        commentsCount = Convert.ToInt32(row["commentsCount"]),
                        userName = row["userName"].ToString(),
                        imgUrl = row["imgUrl"].ToString()
                    });
                }

                return posts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievements for userID: {userId}: {ex.Message}");
                throw;
            }
        }

        public string GetAllPost(string token)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            try
            {
                DataTable result = PostDAL.GetAll();
                if (result == null || result.Rows.Count == 0)
                {
                    return null;
                }
                List<Dictionary<string, object>> posts = new List<Dictionary<string, object>>();
                foreach (DataRow row in result.Rows)
                {
                    var res = PostDAL.GetIsPinnedAndIsReported(Convert.ToInt32(row["postID"]));
                    var postInfo = new Dictionary<string, object> {
                        { "postID" ,Convert.ToInt32(row["postID"]) },
                        { "userID" ,Convert.ToInt32(row["userID"]) },
                        {"postTitle",row["postTitle"].ToString() },
                        { "postContent",row["postContent"].ToString() },
                        {"postCategory",row["postCategory"].ToString() },
                        { "postTime",Convert.ToDateTime(row["postTime"]) },
                        {"likesCount",Convert.ToInt32(row["likesCount"]) },
                        {"forwardCount",Convert.ToInt32(row["forwardCount"]) },
                        { "commentsCount",Convert.ToInt32(row["commentsCount"])},
                        { "userName",row["userName"].ToString() },
                        { "imgUrl",row["imgUrl"].ToString() },
                        { "isPinned",res.isPinned},
                        { "isReported",res.isReported}
                    };
                    posts.Add(postInfo);
                }
                return JsonConvert.SerializeObject(posts, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                throw;
            }
        }

        //var results = postBLL.Search(token, "关键字", category: "技术", dateRange: "2024-01-01,2024-08-01", sortBy: "postTime");
        public List<Post> Search(string token, string query, string category = null, string dateRange = null, string sortBy = "postTime")
        {
            // 验证Token
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (tokenRes == null)
            {
                throw new UnauthorizedAccessException("无效的Token");
            }
            // 调用DAL层的Search方法
            return PostDAL.Search(query, category, dateRange, sortBy);
        }


        public string Delete(string token,int postID, int postUserID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int hostId = PublishDAL.GetHost(postID);
            int userId = tokenRes.userID;
            string role =tokenRes.Role;
            if (hostId != userId && role!="admin")
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "身份权限不符"
                });
            }
            PublishDAL.Delete(postID);
            PostDAL.Delete(postID);
            //更新成就
            _userAchievement.UpdatePostAchievement(postUserID, -1);
            return JsonConvert.SerializeObject(new
            {
                message = "删除帖子成功"
            });
        }

        public string LikePost(string token ,int postID,int postUserID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (PostDAL.Like(postID))
            {
                //通知被点赞方，接入成就系统
                _userAchievement.UpdateBeLiked(postUserID,1);
                var post = PostDAL.GetPostByPostID(postID);
                Message message = new Message()
                {
                    messageID = -1,
                    senderID = tokenRes.userID,
                    receiverID = postUserID,
                    messageType = "点赞帖子",
                    Content = $"{post.postTitle}",
                    sendTime = DateTime.Now,
                    isRead = 0
                };
                MessageDAL.Insert(message);

                return JsonConvert.SerializeObject(new
                {
                    message = "成功点赞帖子"
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "点赞帖子失败"
                });
            }
        }

        public string CancleLike(string token, int postID, int postUserID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (PostDAL.CancleLike(postID))
            {
                _userAchievement.UpdateBeLiked(postUserID, -1);
                return JsonConvert.SerializeObject(new
                {
                    message = "成功取消点赞"
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "取消点赞失败"
                });
            }
        }

        public string Report(string token, int postId) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (PostDAL.Report(postId))
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "成功举报"
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "举报失败"
                });
            }

        }


        public string CancleReport(string token, int postId) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (PostDAL.CancleReport(postId))
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "成功取消举报"
                }) ;
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "取消举报失败"
                });
            }
        }

        public string Pin(string token, int postId)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (PostDAL.Pin(postId))
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "成功置顶"
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "置顶失败"
                });
            }

        }


        public string CanclePin(string token, int postId)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (PostDAL.CanclePin(postId))
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "成功取消置顶"
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "取消置顶失败"
                });
            }
        }

        public string Forward(string token, int postId)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            //Post post = PostDAL.GetByPostID(postId);
            var post = PostDAL.GetPostByPostID(postId);
            int userId = tokenRes.userID;
            int st;
            PostDAL.Forward(postId);
            Post post1 = new Post() {
                postTitle =post.postTitle,
                postContent = post.postContent,
                postCategory = post.postCategory,
                userID =userId, 
                refrencePostID = post.postID,
                userName =UserDAL.GetUserByUserID(userId,out st).userName,
                imgUrl = post.imgUrl
            };
            Post(token,post1);
            //通知
            Message message = new Message()
            {
                messageID = -1,
                senderID = userId,
                receiverID = post.userID,
                messageType = "转发",
                Content = $"{post.postTitle}",
                sendTime = DateTime.Now,
                isRead = 0
            };
            MessageDAL.Insert(message);
            return JsonConvert.SerializeObject(new
            {
                message = "成功转发"
            });
        }

        // 评论区——健身教练AI
        public MessageRes FitCoachComment(string postTitle,string postContent)
        {

            string sys = "- Role: 专业健身教练\r\n" +
                "- Background: 用户在论坛帖子评论区寻求健身指导和建议，希望得到个性化的健身计划和训练方案。\r\n" +
                "- Profile: 你是一位经验丰富的健身教练，专注于为用户提供专业的健身指导和个性化训练计划。你了解不同用户的健身需求和目标，并能提供针对性的建议。\r\n" +
                "- Skills: 你具备运动生理学、营养学和运动心理学的知识，能够根据用户的身体状况、健身目标和生活习惯，设计出科学合理的健身计划。\r\n" +
                "- Goals: 为用户提供专业的健身指导，帮助他们实现健身目标，提高身体素质和健康水平。\r\n" +
                "- Constrains: 评论应保持专业严谨，注重实用性，避免使用过于复杂或专业的术语，确保用户易于理解和执行。\r\n" +
                "- OutputFormat: 提供简洁明了的健身建议和指导，包括训练动作、频率、持续时间、注意事项等。因为只是评论，回复一定要简短！简短！简短！\r\n";


            MessageRes res = new();

            string Prompt = "帖子标题为"+postTitle+",帖子内容为:" +postContent;
            
            res.message = Qwen_VL.CallQWenSingle(sys, Prompt);

            return res;
        }

        // 评论区——营养顾问AI
        public MessageRes NutriExpertComment(string postTitle,string postContent)
        {

            string sys = "- Role: 营养顾问\r\n" +
                "- Background: 用户在论坛帖子评论区寻求饮食营养指导，希望得到专业的饮食建议和营养搭配方案，以支持他们的健康和健身目标。\r\n" +
                "- Profile: 你是一位经验丰富的营养顾问，专注于提供科学的饮食习惯和营养搭配建议。你了解不同用户的饮食习惯和营养需求，并能提供个性化的饮食方案。\r\n" +
                "- Skills: 你具备营养学、食品科学和健康饮食规划的知识，能够根据用户的健康状况、健身目标和口味偏好，设计出健康合理的饮食计划。\r\n" +
                "- Goals: 为用户提供专业的营养指导，帮助他们实现健康饮食，改善身体状况，支持健身目标的达成。\r\n" +
                "- Constrains: 评论应保持温和耐心，强调科学性，注重饮食和营养的重要性，避免使用过于复杂或专业的术语，确保用户易于理解和执行。\r\n" +
                "- OutputFormat: 提供具体的食谱建议、营养搭配方案和饮食指导，包括食材选择、烹饪方法、饮食频率和注意事项等。因为只是评论，回复一定要简短！简短！简短！\r\n";


            MessageRes res = new();

            string Prompt = "帖子的标题为"+ postTitle+"帖子内容为:" + postContent;

            res.message = Qwen_VL.CallQWenSingle(sys, Prompt);

            return res;
        }

        // 评论区——激励导师AI
        public MessageRes MotivatorComment(string postTitle, string postContent)
        {

            string sys = "- Role: 激励导师\r\n" +
                "- Background: 用户在健身旅程中可能会遇到动力不足或挑战困难的时刻，需要激励和支持来保持健身习惯和达成目标。\r\n" +
                "- Profile: 你是一位社区的激励导师，擅长通过正面积极的语言和行动鼓励他人。你了解健身旅程中的挑战，并能提供恰当的激励和支持。\r\n" +
                "- Skills: 你具备心理学、沟通技巧和团队建设的知识，能够通过激励性的话语和活动激发用户的健身动力，促进社区内的互动和互助。\r\n" +
                "- Goals: 帮助用户克服健身过程中的障碍，保持积极态度，鼓励他们坚持健身打卡，参与挑战和活动。\r\n" +
                "- Constrains: 评论应充满正能量，语气热情，使用励志语句激发用户的健身动力，同时避免过度夸张或不切实际的承诺。\r\n" +
                "- OutputFormat: 提供激励性的话语、励志故事、健身挑战和活动信息，以及正面反馈和支持。因为只是评论，一定要简短！简短！简短！\r\n";


            MessageRes res = new();

            string Prompt = "帖子标题为" + postTitle + ",帖子内容为:" + postContent;

            res.message = Qwen_VL.CallQWenSingle(sys, Prompt);

            return res;
        }
    }
}
