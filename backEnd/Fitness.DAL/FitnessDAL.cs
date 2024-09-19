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
    public sealed class FitnessDAL
    {
        private static readonly FitnessDAL instance = new FitnessDAL();
        private FitnessDAL()
        {
        }
        public static FitnessDAL Instance
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
                string insertCommand = "INSERT INTO \"Fitness\" (\"userID\", \"height\", \"weight\", \"BMI\", \"bodyFatRate\")" +
                                    "VALUES (:\"userID\", :\"height\", :\"weight\", :\"BMI\", :\"bodyFatRate\")";

                // 创建参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter(":userID", OracleDbType.Int32) { Value = userId },
                new OracleParameter(":height", OracleDbType.Decimal) { Value = DBNull.Value },
                new OracleParameter(":weight", OracleDbType.Decimal) { Value = DBNull.Value },
                new OracleParameter(":BMI", OracleDbType.Decimal) { Value = DBNull.Value },
                new OracleParameter(":bodyFatRate", OracleDbType.Decimal) { Value = DBNull.Value }
                };

                // 执行插入操作
                OracleHelper.ExecuteNonQuery(insertCommand, null, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
        }

        public static string Update(int userId, double height, double weight, double BMI, double bodyFatRate)
        {
            try
            {
                // 插入UserAchievement的SQL命令
                string updateCommand = "UPDATE \"Fitness\" " +
                                        "SET \"height\" =:height, \"weight\" =:weight, \"BMI\" =:BMI , \"bodyFatRate\" =:bodyFatRate " +
                                        "WHERE \"userID\" =:userID";


                OracleParameter[] parameters = new OracleParameter[]
                {
                        new OracleParameter(":height", OracleDbType.Decimal) { Value = height },
                        new OracleParameter(":weight", OracleDbType.Decimal) { Value = weight },
                        new OracleParameter(":BMI", OracleDbType.Decimal) { Value = BMI },
                        new OracleParameter(":bodyFatRate", OracleDbType.Decimal) { Value = bodyFatRate },
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
            string query = "SELECT \"height\", \"weight\", \"BMI\", \"bodyFatRate\" FROM \"Fitness\" WHERE \"userID\" = :userID";

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
                    Height = (string)null,
                    Weight = (string)null,
                    BMI = (string)null,
                    bodyFatRate = (string)null
                });
            }
            else
            {
                DataRow row = fitnessData.Rows[0];
                return JsonConvert.SerializeObject(new
                {
                    message = "successful",
                    Height = row["height"]?.ToString(),
                    Weight = row["weight"]?.ToString(),
                    BMI = row["BMI"]?.ToString(),
                    bodyFatRate = row["bodyFatRate"]?.ToString()
                });
            }
        }
    }
}