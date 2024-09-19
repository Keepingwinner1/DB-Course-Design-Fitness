using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Fitness.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json;
using Fitness.DAL;
using Fitness.Models;

namespace Fitness.DAL
{
    public sealed class UserAchievementDAL
    {
        private static readonly UserAchievementDAL instance = new UserAchievementDAL();
        private UserAchievementDAL()
        {
        }
        public static UserAchievementDAL Instance
        {
            get
            {
                return instance;
            }
        }

        public static void InitUserAchievementFormUserId_AchievementId(int userId, int achievementId)
        {
            try
            {
                // 插入UserAchievement的SQL命令
                string insertCommand = "INSERT INTO \"UserAchievement\" (\"userID\", \"achievementID\", \"progress\", \"achievedDate\", \"isAchieved\")" +
                                    "VALUES (:\"userID\", :\"achievementID\", :\"progress\", :\"achievedDate\", :\"isAchieved\")";

                // 创建参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                new OracleParameter(":achievementID", OracleDbType.Int32) { Value = achievementId },
                new OracleParameter(":progress", OracleDbType.Int32) { Value = 0 },
                new OracleParameter(":achievedDate", OracleDbType.Date) { Value = DBNull.Value },
                new OracleParameter(":isAchieved", OracleDbType.Int32) { Value = 0 }
                };

                // 执行插入操作
                OracleHelper.ExecuteNonQuery(insertCommand, null, parameters);
                Console.WriteLine($"Inserted UserAchievement for userID: {userId}, achievementID: {achievementId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting UserAchievement for userID: {userId}, achievementID: {achievementId}: {ex.Message}");
            }
        }

        public static void Update(int userId, int achievementId, int goal, bool isAchieved)
        {
            try
            {
                // 插入UserAchievement的SQL命令
                string updateCommand = "UPDATE \"UserAchievement\" " +
                                        "SET \"progress\" =:progress";
                if (isAchieved)
                {
                    updateCommand += ", \"isAchieved\" =:isAchieved" +
                                    ", \"achievedDate\" =:achievedDate";
                }
                updateCommand += " WHERE \"userID\" =:userID AND \"achievementID\" =:achievementID";

                if (isAchieved)
                {
                    OracleParameter[] parameters = new OracleParameter[]
                    {
                        new OracleParameter(":progress", OracleDbType.Int32) { Value = goal },
                        new OracleParameter(":isAchieved", OracleDbType.Int32) { Value = isAchieved ? 1 : 0 },
                        new OracleParameter(":achievedDate", OracleDbType.TimeStamp) { Value = DateTime.Now },
                        new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                        new OracleParameter(":achievementID", OracleDbType.Int32) { Value = achievementId }
                    };

                    // 执行插入操作
                    OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);
                    Console.WriteLine($"Inserted UserAchievement for userID: {userId}, achievementID: {achievementId}");
                }
                else
                {
                    OracleParameter[] parameters = new OracleParameter[]
                    {
                        new OracleParameter(":progress", OracleDbType.Int32) { Value = goal },
                        new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                        new OracleParameter(":achievementID", OracleDbType.Int32) { Value = achievementId }
                    };

                    // 执行插入操作
                    OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);
                    Console.WriteLine($"Inserted UserAchievement for userID: {userId}, achievementID: {achievementId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting UserAchievement for userID: {userId}, achievementID: {achievementId}: {ex.Message}");
            }
        }

        public static int GetProgress(int userId, int achievementId)
        {
            string selectCommand = "SELECT \"progress\" FROM \"UserAchievement\" WHERE \"userID\" = :\"userID\" AND \"achievementID\" = :\"achievementID\"";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                new OracleParameter(":achievementID", OracleDbType.Int32) { Value = achievementId },
            };

            object result = OracleHelper.ExecuteScalar(selectCommand, parameters);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                throw new Exception($"Achievement not found for achievementID: {achievementId}");
            }
        }

        public static bool GetIsAchieved(int userId, int achievementId)
        {
            string selectCommand = "SELECT \"isAchieved\" FROM \"UserAchievement\" WHERE \"userID\" = :\"userID\" AND \"achievementID\" = :\"achievementID\"";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                new OracleParameter(":achievementID", OracleDbType.Int32) { Value = achievementId },
            };

            object result = OracleHelper.ExecuteScalar(selectCommand, parameters);
            if (result != null && result != DBNull.Value)
            {
                int res = Convert.ToInt32(result);
                if (res == 0)
                    return false;
                else
                    return true;
            }
            else
            {
                throw new Exception($"Achievement not found for achievementID: {achievementId}");
            }
        }

        public static DataTable GetUserAchievementTable(int userId)
        {
            string selectCommand =
                "SELECT ua.\"achievementID\", a.\"name\", a.\"target\", ua.\"progress\", ua.\"isAchieved\" " +
                "FROM \"UserAchievement\" ua " +
                "JOIN \"Achievement\" a ON ua.\"achievementID\" = a.\"achievementID\" " +
                "WHERE ua.\"userID\" = :\"userID\" ";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", userId)
            };
            try
            {
                DataTable result = OracleHelper.ExecuteTable(selectCommand, parameters);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching achievements for userID: {userId}: {ex.Message}");
                throw;
            }
        }

        public static DataTable GetRankTable(int achievementId)
        {
            string selectCommand =
                "SELECT ua.\"achievementID\", a.\"name\", a.\"target\", ua.\"progress\", ua.\"isAchieved\", u.\"userID\", u.\"userName\", u.\"iconURL\" " +
                "FROM \"UserAchievement\" ua " +
                "JOIN \"Achievement\" a ON ua.\"achievementID\" = a.\"achievementID\" " +
                "JOIN \"User\" u ON ua.\"userID\" = u.\"userID\" " +
                "WHERE ua.\"achievementID\" = :\"achievementID\" " +
                "ORDER BY ua.\"progress\" DESC, u.\"userID\" " +
                "FETCH FIRST 10 ROWS ONLY";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":achievementID", achievementId)
            };
            DataTable result = OracleHelper.ExecuteTable(selectCommand, parameters);
            return result;
        }

        public static int GetPersonInfoProgress(int userId)
        {
            string query = "SELECT \"Age\", \"Gender\", \"Tags\", \"Introduction\", \"iconURL\", \"goalType\", \"goalWeight\" " +
                   "FROM \"User\" WHERE \"userID\" = :userID";

            // 设置参数
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            // 执行查询
            DataTable resultTable = OracleHelper.ExecuteTable(query, parameters);

            // 如果查询结果为空，返回null
            if (resultTable.Rows.Count == 0)
            {
                return 0;
            }

            // 获取查询结果并赋值给Exercise类
            DataRow row = resultTable.Rows[0];
            int ans = 100;
            if (Convert.ToInt32(row["Age"]) == -1) ans -= 10;
            if (row["Gender"].ToString() == "null") ans -= 10;
            if (row["Tags"].ToString() == "null") ans -= 10;
            if (row["Introduction"].ToString() == "null") ans -= 10;
            if (row["iconURL"].ToString() == "null") ans -= 10;
            if (row["goalType"].ToString() == "null") ans -= 10;
            if (Convert.ToInt32(row["goalWeight"]) == -1) ans -= 10;

            return ans;
        }
    }
}
