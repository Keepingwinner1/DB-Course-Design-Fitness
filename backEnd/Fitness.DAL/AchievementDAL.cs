using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Fitness.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using Fitness.DAL;
using Fitness.Models;

namespace Fitness.DAL
{
    public sealed class AchievementDAL
    {
        public const int _PersonalInfo = 1;
        public const int _LoginDays = 2;
        public const int _CompleteCourse = 3;
        public const int _BeLiked = 4;
        public const int _BeCommented = 5;
        public const int _Post = 6;
        public const int _CompleteDietPlan = 7;
        public const int _CompleteExercisePlan = 8;

        private static readonly AchievementDAL instance = new AchievementDAL();
        private AchievementDAL()
        {
        }
        public static AchievementDAL Instance
        {
            get
            {
                return instance;
            }
        }

        public static DataTable GetAllAchienementId()
        {
            string selectCommand = "SELECT \"achievementID\" FROM \"Achievement\"";
            DataTable achievements = OracleHelper.ExecuteTable(selectCommand);
            return achievements;
        }

        public static int GetAchievementTarget(int achievementId)
        {
            // 查询 Achievement 表中特定成就的 target 值
            string selectCommand = "SELECT \"target\" FROM \"Achievement\" WHERE \"achievementID\" = :\"achievementID\"";
            OracleParameter parameter = new OracleParameter(":achievementID", achievementId);

            object result = OracleHelper.ExecuteScalar(selectCommand, parameter);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                throw new Exception($"Achievement not found for achievementID: {achievementId}");
            }
        }

    }
}
