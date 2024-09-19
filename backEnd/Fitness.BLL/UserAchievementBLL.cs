using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.DAL.Core;
using Newtonsoft.Json;
using Fitness.DAL;
using Fitness.BLL;
using Fitness.Models;
using Fitness.BLL.Interfaces;

namespace Fitness.BLL
{
    public class UserAchievementBLL : IUserAchievementBLL
    {
        VigorTokenBLL vigorTokenBLL = new();
        public void Init(int userId)
        {
            try
            {
                // 获取所有成就的ID
                DataTable achievements = AchievementDAL.GetAllAchienementId();

                if (achievements == null || achievements.Rows.Count == 0)
                {
                    Console.WriteLine("No achievements found.");
                    return;
                }

                // 遍历所有成就ID，将其与用户ID配对插入UserAchievement表
                foreach (DataRow row in achievements.Rows)
                {
                    if (row["achievementID"] == DBNull.Value)
                    {
                        Console.WriteLine("Found a null achievementID, skipping...");
                        continue;
                    }

                    int achievementId = Convert.ToInt32(row["achievementID"]);
                    //Console.WriteLine(achievementId);
                    UserAchievementDAL.InitUserAchievementFormUserId_AchievementId(userId, achievementId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing user achievements: {ex.Message}");
            }
        }

        public void UpdateUserAchievement(int userId, int achievementId, int goal = 1)
        {
            int target = AchievementDAL.GetAchievementTarget(achievementId);
            bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, achievementId);
            int nowprogress = UserAchievementDAL.GetProgress(userId, achievementId);
            if (achievementId == AchievementDAL._PersonalInfo)
            {
                if (goal >= target && isAchieved == false)
                {
                    vigorTokenBLL.UpdateBalance(userId, "完成成就，获得200活力币", 200);
                    UserAchievementDAL.Update(userId, achievementId, goal, true);
                }
                else
                {
                    //vigorTokenBLL.UpdateBalance(userId, "刷新成就进度，获得100活力币", 100);
                    UserAchievementDAL.Update(userId, achievementId, goal, false);
                }
            }
            else
            {
                if (nowprogress + 1 == target && isAchieved == false)
                {
                    vigorTokenBLL.UpdateBalance(userId, "完成成就，获得200活力币", 200);
                    UserAchievementDAL.Update(userId, achievementId, nowprogress + goal, true);
                }
                else
                {
                    //vigorTokenBLL.UpdateBalance(userId, "刷新成就进度，获得100活力币", 100);
                    UserAchievementDAL.Update(userId, achievementId, nowprogress + goal, false);
                }
                    
            }
        }

        public string GetUserAchievement(int userId)
        {
            try
            {
                DataTable result = UserAchievementDAL.GetUserAchievementTable(userId);
                if (result == null || result.Rows.Count == 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        totAchievements = 0,
                        gotAchievements = 0,
                        achievements = new List<object>()
                    });
                }

                int totalAchievements = result.Rows.Count;
                int gotAchievements = 0;

                var achievements = new List<object>();
                foreach (DataRow row in result.Rows)
                {
                    bool isAchieved = Convert.ToInt32(row["isAchieved"]) == 1;
                    if (isAchieved)
                    {
                        gotAchievements++;
                    }

                    achievements.Add(new
                    {
                        achievementId = row["achievementID"].ToString(),
                        name = row["name"].ToString(),
                        target = row["target"].ToString(),
                        progress = row["progress"].ToString(),
                        isAchieved = isAchieved.ToString().ToLower()
                    });
                }

                var response = new
                {
                    totAchievements = totalAchievements.ToString(),
                    gotAchievements = gotAchievements.ToString(),
                    achievements = achievements
                };
                Console.WriteLine("收到获取成就请求");
                return JsonConvert.SerializeObject(response, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievements for userID: {userId}: {ex.Message}");
                throw;
            }
        }

        public string GetAchievementRank(int userId, int achievementId)
        {
            try
            {
                DataTable result = UserAchievementDAL.GetRankTable(achievementId);
                if (result == null || result.Rows.Count == 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        myRanking = -1,
                        progress = 0,
                        totRanking = 0,
                        rankingUsers = new List<object>()
                    });
                }

                var rankingUsers = new List<object>();
                int myRanking = -1;
                int progress = 0;
                int totRanking = result.Rows.Count;

                int rank = 1;
                foreach (DataRow row in result.Rows)
                {
                    int currentUserId = Convert.ToInt32(row["userID"]);
                    int currentProgress = Convert.ToInt32(row["progress"]);

                    if (currentUserId == userId)
                    {
                        myRanking = rank;
                        progress = currentProgress;
                    }

                    rankingUsers.Add(new
                    {
                        userID = currentUserId.ToString(),
                        userName = row["userName"].ToString(),
                        userIcon = row["iconURL"].ToString(),
                        userRank = rank.ToString(),
                        progress = currentProgress.ToString()
                    });

                    rank++;
                }

                var response = new
                {
                    myRanking = myRanking.ToString(),
                    progress = progress.ToString(),
                    totRanking = totRanking.ToString(),
                    rankingUsers = rankingUsers
                };
                Console.WriteLine("返回排行榜信息如下");
                Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
                return JsonConvert.SerializeObject(response, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievement ranking for userID: {userId}, achievementID: {achievementId}: {ex.Message}");
                throw;
            }
        }

        public bool UpdateUserInfoAchievement(int userId)
        {
            try
            {
                int goal = UserAchievementDAL.GetPersonInfoProgress(userId);
                bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 1);
                UpdateUserAchievement(userId, 1, goal);
                if(isAchieved == false)
                    vigorTokenBLL.UpdateBalance(userId, "完善个人信息，获得50活力币", 50);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateLoginAchievement(int userId)
        {
            try
            {
                //vigorTokenBLL.UpdateBalance(userId, "每日登录，获得188活力币", 188);
                UpdateUserAchievement(userId, 2);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool UpdateCompleteCourse(int userId)
        {
            try
            {
                bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 3);
                if (isAchieved == false)
                    vigorTokenBLL.UpdateBalance(userId, "完成课程，获得100活力币", 100);
                UpdateUserAchievement(userId, 3);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateBeLiked(int userId, int goal)
        {
            try
            {
                bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 4);
                if (isAchieved == false)
                    vigorTokenBLL.UpdateBalance(userId, "被点赞，获得18活力币", 18);
                UpdateUserAchievement(userId, 4, goal);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool UpdateBeComment(int userId, int goal)
        {
            try
            {
                bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 5);
                if (isAchieved == false)
                    vigorTokenBLL.UpdateBalance(userId, "被评论，获得50活力币", 50);
                UpdateUserAchievement(userId, 5, goal);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool UpdatePostAchievement(int userId, int goal)
        {
            try
            {
                bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 6);
                if (isAchieved == false)
                    vigorTokenBLL.UpdateBalance(userId, "成功发帖，获得100活力币", 100);
                UpdateUserAchievement(userId, 6, goal);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool UpdateFoodPlanAchievement(int userId)
        {
            try
            {
                bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 7);
                if (isAchieved == false)
                    vigorTokenBLL.UpdateBalance(userId, "完成饮食计划，获得100活力币", 100);
                UpdateUserAchievement(userId, 7);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public string UpdateFitnessPlanAchievement(int userId, int date)
        {
            UpdateUserAchievement(userId, 8);
            bool isAchieved = UserAchievementDAL.GetIsAchieved(userId, 8);
            if (isAchieved == false)
                vigorTokenBLL.UpdateBalance(userId, "完成健身计划，获得100活力币", 100);
            UserFitnessPlanDAL.CompletePlan(userId, date);
            return JsonConvert.SerializeObject(new
            {
                message = "successful"
            });
        }
    }
}
