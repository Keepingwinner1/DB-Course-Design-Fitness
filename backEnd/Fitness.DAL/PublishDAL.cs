using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.Models;
using Fitness.DAL.Core;
using System.Data;

namespace Fitness.DAL
{
    public class PublishDAL
    {
        public static void Post(Publish publish)
        {
            string query = "INSERT INTO \"Publish\" " +
                             "(\"userID\", \"postID\", \"publishTime\") " +
                             "VALUES " +
                             "(:userID, :postID, :publishTime) ";
            //Console.WriteLine(publish.postID);
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("userID", OracleDbType.Int32) { Value = publish.userID },
                new OracleParameter("postID", OracleDbType.Int32) { Value = publish.postID },
                new OracleParameter("publishTime", OracleDbType.TimeStamp) { Value = publish.publishTime },
            };

            try
            {
                OracleHelper.ExecuteNonQuery(query, null, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting post: {ex.Message}");
                throw;
            }
        }

        public static int GetHost(int postId)
        {
            //Console.WriteLine(postId);
            string selectCommand = "SELECT \"userID\" FROM \"Publish\" WHERE \"postID\" = :postID";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
            };

            DataTable dt = OracleHelper.ExecuteTable(selectCommand, parameters);
            //Console.WriteLine(dt.Rows.Count);

            if (dt.Rows.Count > 0)
            {
                // 使用字段名称时需要确保其大小写与数据库一致
                object userIDValue = dt.Rows[0]["userID"];

                // 判断是否能成功转换为整数类型
                if (userIDValue != DBNull.Value && int.TryParse(userIDValue.ToString(), out int userID))
                {
                    return userID;
                }
            }

            // 若无结果或无法转换，则返回-1表示无效的userID
            return -1;
        }

        public static bool Delete(int postId)
        {
            try
            {
                string deleteCommand = "DELETE FROM \"Publish\" WHERE \"postID\" = :postID";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(deleteCommand, null, parameters);

                // 如果删除操作影响了行数，返回 true，表示删除成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post with postID {postId}: {ex.Message}");
                return false;
            }
        }
    }
}