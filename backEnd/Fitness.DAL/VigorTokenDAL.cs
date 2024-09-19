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
    public class VigorTokenDAL
    {
        // 获取活力币余额
        public DataTable GetVigorTokenBalance(int userID)
        {
            try
            {
                string sql = "SELECT \"vigorTokenBalance\" FROM \"User\" WHERE \"userID\"=:\"userID\"";
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

        public bool UpdateVigorTokenBalance(int change, int userID)
        {
            try
            {
                string sql = "UPDATE \"User\" SET \"vigorTokenBalance\"=\"vigorTokenBalance\"+:change " +

                                "WHERE \"userID\"=:userID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("change", OracleDbType.Int32) { Value = change },
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }

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

        // 插入活力币变化表
        public bool InsertVigorTokenRecord(VigorTokenInfo vigorTokenInfo)
        {
            try
            {
                string sql = "INSERT INTO \"VigorTokenRecord\" (\"userID\", \"reason\", \"change\", \"balance\", \"createTime\")" +
                             " VALUES (:\"userID\", :\"reason\", :\"change\", :\"balance\", :\"createTime\")";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = vigorTokenInfo.userID },
                    new OracleParameter("\"reason\"", OracleDbType.Clob) { Value = vigorTokenInfo.reason },
                    new OracleParameter("\"change\"", OracleDbType.Int32) { Value = vigorTokenInfo.change },
                    new OracleParameter("\"balance\"", OracleDbType.Int32) { Value = vigorTokenInfo.balancce },
                    new OracleParameter("\"createTime\"", OracleDbType.TimeStamp) { Value = vigorTokenInfo.createTime }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting VigorTokenRecord: {ex.Message}");
                throw;
            }
        }

        // 获取用户的所有的活力币记录
        public DataTable GetVigorTokenRecordsByUsrID(int userID)
        {
            try
            {
                string sql = "SELECT * FROM \"VigorTokenRecord\" WHERE \"userID\"=:\"userID\"";
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

    }
}
