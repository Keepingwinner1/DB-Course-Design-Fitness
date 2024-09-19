using Fitness.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IUserAchievementBLL
    {
        public void Init(int userId);

        public void UpdateUserAchievement(int userId, int achievementId, int goal = 1);

        public string GetUserAchievement(int userId);

        public string GetAchievementRank(int userId, int achievementId);

        public bool UpdateUserInfoAchievement(int userId);

        public bool UpdateLoginAchievement(int userId);

        public bool UpdateCompleteCourse(int userId);

        public bool UpdateBeLiked(int userId, int goal);

        public bool UpdateBeComment(int userId, int goal);

        public bool UpdatePostAchievement(int userId, int goal);

        public bool UpdateFoodPlanAchievement(int userId);

        public string UpdateFitnessPlanAchievement(int userId, int date);
    }
}
