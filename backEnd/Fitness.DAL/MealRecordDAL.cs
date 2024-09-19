using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class MealRecordDAL
    {
        // 成功插入一条饮食记录时，返回这条饮食记录的recordID
        public int InsertMealRecord(MealRecordInfo mealRecordInfo, OracleTransaction transaction=null)
        {
            int recordID = -1;

            try
            {
                string sql = "INSERT INTO \"MealRecords\" (\"userID\", \"createTime\", \"mealTime\", \"mealPhoto\")" +
                    " VALUES (:\"userID\", :\"createTime\", :\"mealTime\", :\"mealPhoto\")" +
                    " RETURNING \"recordID\" INTO :\"recordID\"";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = mealRecordInfo.userID },
                    new OracleParameter("\"createTime\"", OracleDbType.TimeStamp) { Value = DateTime.Now },
                    new OracleParameter("\"mealTime\"", OracleDbType.TimeStamp) { Value = mealRecordInfo.mealTime },
                    new OracleParameter("\"mealPhoto\"", OracleDbType.Clob) { Value = mealRecordInfo.mealPhoto },
                    new OracleParameter("\"recordID\"", OracleDbType.Int32) { Direction = ParameterDirection.Output }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                recordID = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)oracleParameters[4].Value).Value);

                return recordID;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting meal record: {ex.Message}");
                throw;
            }
        }

        public bool InsertMealRecordsFood(MealRecordFood mealRecordFood,OracleTransaction transaction=null)
        {
            try
            {
                string sql = "INSERT INTO \"MealRecordsFood\"(\"recordID\", \"foodName\", \"foodAmount\") " +
                             "VALUES(:\"recordID\", :\"foodName\", :\"foodAmount\")";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"recordID\"", OracleDbType.Int32) { Value = mealRecordFood.recordID },
                    new OracleParameter("\"foodName\"", OracleDbType.NVarchar2) { Value = mealRecordFood.foodName },
                    new OracleParameter("\"foodAmount\"", OracleDbType.Int32) { Value = mealRecordFood.foodAmount }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food plan food: {ex.Message}");
                throw;
            }
        }

        // 待大模型生成饮食建议后更新数据库
        public bool UpdateAdvicing(int recordID, string mealAdvice)
        {

            try
            {
                string sql = "UPDATE \"MealRecords\" SET \"diningAdvice\"=:diningAdvice " +
                                "WHERE \"recordID\"=:recordID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("diningAdvice", OracleDbType.Clob) { Value = mealAdvice },
                    new OracleParameter("recordID", OracleDbType.Int32) { Value = recordID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public DataTable GetMealRecordByUsrID(int userID)
        {
            try
            {
                string sql = "SELECT * FROM \"MealRecords\" WHERE \"userID\"=:\"userID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = userID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable GetMealRecordByIDAndDate(int userID,DateTime date)
        {
            try
            {
                string sql = "SELECT * FROM \"MealRecords\" WHERE \"userID\"=:\"userID\" AND TRUNC(\"mealTime\")=:\"date\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = userID },
                    new OracleParameter("\"date\"", OracleDbType.Date) { Value = date.Date });

                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable GetMealRecordsFoodsByID(int recordID)
        {
            try
            {
                string sql = "SELECT * FROM \"MealRecordsFood\" WHERE \"recordID\"=:\"recordID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"recordID\"", OracleDbType.Int32) { Value = recordID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public DataTable GetMealRecordByRecordID(int recordID)
        {
            try
            {
                string sql = "SELECT * FROM \"MealRecords\" WHERE \"recordID\"=:\"recordID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"recordID\"", OracleDbType.Int32) { Value = recordID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable GetAISuggestionByRecordID(int recordID)
        {
            try
            {
                string sql = "SELECT \"diningAdvice\" FROM \"MealRecords\" WHERE \"recordID\"=:\"recordID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"recordID\"", OracleDbType.Int32) { Value = recordID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public bool CheckFoodExists(string foodName)
        {
            try
            {
                // SQL 查询语句，使用参数化查询防止SQL注入
                string sql = "SELECT COUNT(*) FROM \"Foods\" WHERE \"foodName\" = :foodName";

                // 创建参数并赋值
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
            new OracleParameter(":foodName", foodName)
                };

                // 执行查询并获取结果
                object result = OracleHelper.ExecuteScalar(sql, oracleParameters);

                // 将结果转换为整数并检查是否大于0
                int count = Convert.ToInt32(result);

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public DataTable GetOneFoodCalorie(string foodName)
        {
            try
            {
                string sql = "SELECT \"calorie\" FROM \"Foods\" WHERE \"foodName\" = :foodName ";
                OracleParameter[] oracleParameters = new OracleParameter[] 
                {
                    new OracleParameter("foodName", OracleDbType.Clob) { Value = foodName }
                };
                DataTable dt = OracleHelper.ExecuteTable(sql, oracleParameters);


                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public bool DeleteMealRecordByRecordID(int recordID)
        {
            try
            {
                string sql = "DELETE FROM \"MealRecords\" WHERE \"recordID\" =:recordID";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("recordID", OracleDbType.Int32) { Value = recordID }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeletMealRecordFoodByID(int recordID, OracleTransaction transaction = null)
        {
            try
            {
                string sql = "DELETE FROM \"MealRecordsFood\" WHERE \"recordID\" =:\"recordID\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("recordID", OracleDbType.Int32) { Value = recordID }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public bool UpdateMealRecordByRecordID(int recordID, DateTime mealTime, string mealPhoto,OracleTransaction transaction = null)
        {

            try
            {
                string sql = "UPDATE \"MealRecords\" SET \"mealTime\"=:mealTime, \"mealPhoto\"=:mealPhoto " +
                                "WHERE \"recordID\"=:recordID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("mealTime", OracleDbType.TimeStamp) { Value = mealTime },
                    new OracleParameter("mealPhoto", OracleDbType.Clob) { Value = mealPhoto },
                    new OracleParameter("recordID", OracleDbType.Int32) { Value = recordID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }




    }
}
