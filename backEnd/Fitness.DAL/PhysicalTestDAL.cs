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
    public sealed class PhysicalTestDAL
    {
        private static readonly PhysicalTestDAL instance = new PhysicalTestDAL();
        private PhysicalTestDAL()
        {
        }
        public static PhysicalTestDAL Instance
        {
            get
            {
                return instance;
            }
        }

        public static void Init(int userId)
        {
            try
            {
                // 插入UserAchievement的SQL命令
                string insertCommand = "INSERT INTO \"PhysicalTest\" (\"userID\", \"pushups\", \"squats\", \"situps\", \"pullups\", \"longDistance\")" +
                                    "VALUES (:\"userID\", :\"pushups\", :\"squats\", :\"situps\", :\"pullups\", :\"longDistance\")";

                // 创建参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                    new OracleParameter(":pushups", OracleDbType.Int32) { Value = DBNull.Value },
                    new OracleParameter(":squats", OracleDbType.Int32) { Value = DBNull.Value },
                    new OracleParameter(":situps", OracleDbType.Int32) { Value = DBNull.Value },
                    new OracleParameter(":pullups", OracleDbType.Int32) { Value = DBNull.Value },
                    new OracleParameter(":longDistance", OracleDbType.Int32) { Value = DBNull.Value }
                };

                // 执行插入操作
                OracleHelper.ExecuteNonQuery(insertCommand, null, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
        }

        public static string Update(int userId, int pushups, int squats, int situps, int pullups, int longDistance)
        {
            try
            {
                // 插入UserAchievement的SQL命令
                string updateCommand = "UPDATE \"PhysicalTest\" " +
                                        "SET \"pushups\" =:pushups, \"squats\" =:squats, \"situps\" =:situps , \"pullups\" =:pullups, \"longDistance\" =:longDistance " +
                                        "WHERE \"userID\" =:userID";


                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":pushups", OracleDbType.Int32) { Value = pushups },
                    new OracleParameter(":squats", OracleDbType.Int32) { Value = squats },
                    new OracleParameter(":situps", OracleDbType.Int32) { Value = situps },
                    new OracleParameter(":pullups", OracleDbType.Int32) { Value = pullups },
                    new OracleParameter(":longDistance", OracleDbType.Int32) { Value = longDistance },
                    new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                };

                // 执行插入操作
                OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);
                return "Successful";
            }
            catch (Exception ex)
            {
                return ex.Message;
                Console.WriteLine($"{ex.Message}");
            }
        }

        public static string Get(int userId)
        {
            string query = "SELECT \"pushups\", \"squats\", \"situps\", \"pullups\", \"longDistance\" FROM \"PhysicalTest\" WHERE \"userID\" = :userID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId }
            };

            DataTable fitnessData = OracleHelper.ExecuteTable(query, parameters);

            if (fitnessData.Rows.Count == 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    message = "empty",
                    pushups = (string)null,
                    squats = (string)null,
                    situps = (string)null,
                    pullups = (string)null,
                    longDistance = (string)null
                });
            }
            else
            {
                DataRow row = fitnessData.Rows[0];
                return JsonConvert.SerializeObject(new
                {
                    message = "successful",
                    pushups = row["pushups"]?.ToString(),
                    squats = row["squats"]?.ToString(),
                    situps = row["situps"]?.ToString(),
                    pullups = row["pullups"]?.ToString(),
                    longDistance = row["longDistance"]?.ToString()
                });
            }
        }
    }
}