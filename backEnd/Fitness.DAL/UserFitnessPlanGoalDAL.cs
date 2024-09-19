using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.DAL.Core;
using Newtonsoft.Json;
using System.Data;
using Fitness.DAL;
using Fitness.Models;

namespace Fitness.DAL
{
    public sealed class UserFitnessPlanGoalDAL
    {
        private static readonly UserFitnessPlanGoalDAL instance = new UserFitnessPlanGoalDAL();
        private UserFitnessPlanGoalDAL()
        {
        }
        public static UserFitnessPlanGoalDAL Instance
        {
            get
            {
                return instance;
            }
        }

        public static bool Exist(int userId)
        {
            string query = "SELECT COUNT(1) FROM \"UserFitnessPlanGoal\" WHERE \"userID\" = :userID";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };
            object result = OracleHelper.ExecuteScalar(query, parameters);
            // 如果查询结果为 1 或更大值，则说明用户存在
            return Convert.ToInt32(result) > 0;
        }

        public static void Init(int userId)
        {
            try
            {
                // 插入UserAchievement的SQL命令
                string insertCommand = "INSERT INTO \"UserFitnessPlanGoal\" (\"userID\", \"duration\", \"planType\")" +
                                    "VALUES (:\"userID\", :\"duration\", :\"planType\")";

                // 创建参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                new OracleParameter(":duration", OracleDbType.Int32) { Value =  DBNull.Value },
                new OracleParameter(":planType", OracleDbType.NVarchar2) { Value =  DBNull.Value },
                };

                // 执行插入操作
                OracleHelper.ExecuteNonQuery(insertCommand, null, parameters);
                //Console.WriteLine($"Inserted UserAchievement for userID: {userId}, achievementID: {achievementId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public static string Get(int userId)
        {
            string query = "SELECT \"planType\", \"duration\" FROM \"UserFitnessPlanGoal\" WHERE \"userID\" = :userID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            DataTable goalData = OracleHelper.ExecuteTable(query, parameters);

            if (goalData.Rows.Count == 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "fail",
                    goal = (string)null,
                    duration = (string)null
                });
            }
            else
            {
                DataRow row = goalData.Rows[0];
                if(String.IsNullOrEmpty(row["planType"]?.ToString()) || String.IsNullOrEmpty(row["duration"]?.ToString()))
                    return JsonConvert.SerializeObject(new
                    {
                        message = "fail",
                        goal = (string)null,
                        duration = (string)null
                    });
                return JsonConvert.SerializeObject(new
                {
                    message = "successful",
                    goal = row["planType"]?.ToString(),
                    duration = row["duration"]?.ToString()
                });
            }
        }

        public static void Update(int userId, string goal, int duration)
        {
            string query = "UPDATE \"UserFitnessPlanGoal\" " +
                             "SET \"planType\" = :goal, \"duration\" = :duration " +
                             "WHERE \"userID\" = :userID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":goal", OracleDbType.NVarchar2) { Value = goal },
                new OracleParameter(":duration", OracleDbType.Int32) { Value = duration },
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            try
            {
                int rowsAffected = OracleHelper.ExecuteNonQuery(query, null, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating UserFitnessPlanGoal: {ex.Message}");
            }
        }

        public static string GetGoal(int userId)
        {
            string query = "SELECT \"planType\" FROM \"UserFitnessPlanGoal\" WHERE \"userID\" = :userID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            DataTable goalData = OracleHelper.ExecuteTable(query, parameters);
            return goalData.Rows[0]["planType"].ToString();
        }
    }
}
