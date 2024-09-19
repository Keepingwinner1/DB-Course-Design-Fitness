using Fitness.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public sealed class UserFitnessPlanDAL
    {
        private static readonly UserFitnessPlanDAL instance = new UserFitnessPlanDAL();
        private UserFitnessPlanDAL()
        {
        }
        public static UserFitnessPlanDAL Instance
        {
            get
            {
                return instance;
            }
        }

        static List<string> loseWeight = new List<string> { "HIIT燃脂", "上肢力量初级", "HIIT燃脂", "告别驼背计划", "HIIT燃脂", "告别粗腿", "HIIT燃脂", "燃胸计划" };

        static List<string> buildMuscle = new List<string> { "轰炸麒麟臂", "告别驼背计划", "腹肌撕裂", "腹肌撕裂", "上肢力量初级", "背部增肌训练", "告别粗腿", "臀腿雕刻", "铠甲胸肌", "铠甲胸肌", "燃胸计划" };

        static List<string> bodySculpt = new List<string> { "HIIT燃脂", "上肢力量初级", "腹肌撕裂", "告别驼背计划", "燃胸计划", "告别粗腿", "HIIT燃脂", "臀腿雕刻" };

        public static void GeneratePlan(int userId, string goal)
        {
            List<string> planList = new List<string>();
            if (goal == "loseWeight") planList = loseWeight;
            else if (goal == "buildMuscle") planList = buildMuscle;
            else if (goal == "bodySculpting") planList = bodySculpt;
            Random random = new Random();
            //string insert = "INSERT INTO \"Fitness\" (\"userID\", \"height\", \"weight\", \"BMI\", \"bodyFatRate\")" +
            //                        "VALUES (:\"userID\", :\"height\", :\"weight\", :\"BMI\", :\"bodyFatRate\")";
            string insertCommand = "INSERT INTO \"UserFitnessPlan\" (\"userID\", \"workoutName\", \"date\", \"isCompleted\") VALUES (:\"userID\", :\"workoutName\", :\"date\", :\"isCompleted\")";

            for (int i = 0; i < 28; i++)
            {
                // 随机选择一个 workoutName
                string randomWorkoutName = planList[random.Next(planList.Count)];

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                    new OracleParameter(":workoutName", OracleDbType.NVarchar2) { Value = randomWorkoutName },
                    new OracleParameter(":date", OracleDbType.Int32) { Value = i },
                    new OracleParameter(":isCompleted", OracleDbType.Int32) { Value = 0 }
                };

                OracleHelper.ExecuteNonQuery(insertCommand, null, parameters);
            }
        }

        public static DataTable GetPlan(int userId)
        {
            string selectCommand = "SELECT \"workoutName\", \"date\", \"isCompleted\" FROM \"UserFitnessPlan\" WHERE \"userID\" = :userID ORDER BY \"date\" ASC";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            DataTable result = OracleHelper.ExecuteTable(selectCommand, parameters);
            if(result.Rows.Count == 0)
                GeneratePlan(userId, UserFitnessPlanGoalDAL.GetGoal(userId));
            result = OracleHelper.ExecuteTable(selectCommand, parameters);
            return result;
        }

        public static void CompletePlan(int userId, int date)
        {
            string updateCommand = "UPDATE \"UserFitnessPlan\" SET \"isCompleted\" = :\"isCompleted\" WHERE \"userID\" = :\"userID\" AND \"date\" = :\"date\"";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":isCompleted", OracleDbType.Int32) { Value = 1 },
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                new OracleParameter(":date", OracleDbType.Int32) { Value = date }
            };

            OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);
        }

        public static void DeletePlan(int userId)
        {
            string deleteCommand = "DELETE FROM \"UserFitnessPlan\" WHERE \"userID\" = :userID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            OracleHelper.ExecuteNonQuery(deleteCommand, null, parameters);
        }
    }
}
