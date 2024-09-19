using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Fitness.DAL;
using Fitness.BLL;
using Fitness.Models;
using Fitness.BLL.Core;

namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        UserAchievementBLL userAchievementBll = new UserAchievementBLL();
        private readonly JWTHelper _jwtHelper = new();
        [HttpGet]
        public string GetAchievement(string token,int userID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userId = userID;
            Console.WriteLine($"收到用户{userID}的获取成就进度请求");
            string result = userAchievementBll.GetUserAchievement(userId);
            Console.WriteLine(result);
            return result;
        }

        [HttpGet]
        public string GetAchievementRanking(string token, int achievementId)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userId = tokenRes.userID;
            Console.WriteLine($"收到用户：{userId} 的获取成就 {achievementId} 排行榜请求");
            return userAchievementBll.GetAchievementRank(userId, achievementId);
        }

        [HttpGet]
        public string UpdateFitnessPlanAchievement(string token, int workoutIndex)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userId = tokenRes.userID;
            Console.WriteLine($"收到用户：{userId} 的完成健身计划请求，date：{workoutIndex}");
            return userAchievementBll.UpdateFitnessPlanAchievement(userId, workoutIndex);
        }
    }
}
