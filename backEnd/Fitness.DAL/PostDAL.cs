using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.Models;
using Fitness.DAL.Core;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using System.Runtime.CompilerServices;

namespace Fitness.DAL
{
    public class PostDAL
    {
        private  static List<Post> ToModelList(DataTable dt)
        {
            List<Post> posts = new List<Post>();

            foreach (DataRow row in dt.Rows)
            {
                Post post = new Post
                {
                    postID = Convert.ToInt32(row["postID"]),
                    userID = Convert.ToInt32(row["userID"]),
                    postTitle = row["postTitle"].ToString(),
                    postContent = row["postContent"].ToString(),
                    postCategory = row["postCategory"].ToString(),
                    postTime = Convert.ToDateTime(row["postTime"]),
                    likesCount = Convert.ToInt32(row["likesCount"]),
                    forwardCount = Convert.ToInt32(row["forwardCount"]),
                    commentsCount = Convert.ToInt32(row["commentsCount"]),
                    refrencePostID = Convert.ToInt32(row["refrencePostID"]),
                    userName = row["userName"].ToString(),
                    imgUrl = row["imgUrl"].ToString()
                };
                posts.Add(post);
            }

            return posts;
        }


        public static int Post(Post post)
        {
            string query = "INSERT INTO \"Posts\" " +
                             "(\"userID\", \"postTitle\", \"postContent\", \"postCategory\", \"postTime\", \"likesCount\", \"forwardCount\", \"commentsCount\",\"refrencePostID\",\"userName\",\"imgUrl\",\"isPinned\",\"isReported\") " +
                             "VALUES "+
                             "(:userID, :postTitle, :postContent, :postCategory, :postTime, :likesCount, :forwardCount, :commentsCount,:refrencePostID, :userName, :imgUrl, :isPinned,:isReported) " +
                             "RETURNING \"postID\" INTO :postID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("userID", OracleDbType.Int32) { Value = post.userID },
                new OracleParameter("postTitle", OracleDbType.NVarchar2) { Value = post.postTitle },
                new OracleParameter("postContent", OracleDbType.NVarchar2) { Value = post.postContent },
                new OracleParameter("postCategory", OracleDbType.NVarchar2) { Value = post.postCategory },
                new OracleParameter("postTime", OracleDbType.TimeStamp) { Value = post.postTime },
                new OracleParameter("likesCount", OracleDbType.Int32) { Value = post.likesCount },
                new OracleParameter("forwardCount", OracleDbType.Int32) { Value = post.forwardCount },
                new OracleParameter("commentsCount", OracleDbType.Int32) { Value = post.commentsCount },
                new OracleParameter("refrencePostID", OracleDbType.Int32) { Value = post.refrencePostID },
                new OracleParameter("userName",OracleDbType.NVarchar2){Value =post.userName},
                new OracleParameter("imgUrl",OracleDbType.Clob){Value =post.imgUrl},
                new OracleParameter("isPinned", OracleDbType.Int32) { Value =0},
                new OracleParameter("isReported", OracleDbType.Int32) { Value = 0},
                new OracleParameter("postID", OracleDbType.Int32, ParameterDirection.Output)
            };

            try
            {
                OracleHelper.ExecuteNonQuery(query, null, parameters);
                OracleDecimal  oracleInt = (OracleDecimal)parameters[13].Value;
                return oracleInt.ToInt32();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting post: {ex.Message}");
                return 0;
            }
        }

        public static state GetIsPinnedAndIsReported(int postID) {
            string selectCommand = "SELECT \"isPinned\",\"isReported\" FROM \"Posts\" WHERE \"postID\" = :postID ";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("postID", OracleDbType.Int32) { Value = postID }
            };
            var dt = OracleHelper.ExecuteTable(selectCommand, parameters);
            return new state()
            {
                isPinned = Convert.ToInt32(dt.Rows[0]["isPinned"]),
                isReported = Convert.ToInt32(dt.Rows[0]["isReported"])
            };
        }

        public static DataTable GetByPostID(int postID)
        {
            string selectCommand = "SELECT * FROM \"Posts\" WHERE \"postID\" = :postID ";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("postID", OracleDbType.Int32) { Value = postID }
            };
            return OracleHelper.ExecuteTable(selectCommand, parameters); ;
        }

        public static DataTable GetByUserID(int userId)
        {
            string selectCommand = "SELECT * FROM \"Posts\" WHERE \"userID\" = :userID ORDER BY \"postTime\" DESC";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("userID", OracleDbType.Int32) { Value = userId }
            };
            return OracleHelper.ExecuteTable(selectCommand, parameters);;
        }

        public static DataTable GetAll()
        {
            string selectCommand = "SELECT * FROM \"Posts\" ";
            return OracleHelper.ExecuteTable(selectCommand);
        }

        public static List<Post> Search(string query, string category = null, string dateRange = null, string sortBy = "postTime")
        {
            // 构建基础SQL查询语句
            string sql = "SELECT * FROM \"Posts\" WHERE \"postContent\" LIKE :query";
            List<OracleParameter> parameters = new List<OracleParameter>
            {
                new OracleParameter("query", OracleDbType.NVarchar2) { Value = "%" + query + "%" }
            };

            // 添加类别过滤条件
            if (!string.IsNullOrEmpty(category))
            {
                sql += " AND \"postCategory\" = :category";
                parameters.Add(new OracleParameter("category", OracleDbType.NVarchar2) { Value = category });
            }

            // 添加时间范围过滤条件
            if (!string.IsNullOrEmpty(dateRange))
            {
                sql += " AND \"postTime\" BETWEEN :startDate AND :endDate";
                var dates = dateRange.Split(',');
                parameters.Add(new OracleParameter("startDate", OracleDbType.Date) { Value = DateTime.Parse(dates[0]) });
                parameters.Add(new OracleParameter("endDate", OracleDbType.Date) { Value = DateTime.Parse(dates[1]) });
            }

            // 添加排序条件
            if (!string.IsNullOrEmpty(sortBy))
            {
                sql += $" ORDER BY \"{sortBy}\" DESC";
            }

            // 执行SQL查询并返回结果
            DataTable dt = OracleHelper.ExecuteTable(sql, parameters.ToArray());
            return ToModelList(dt);
        }


        public static bool Delete(int postId)
        {
            try
            {
                string deleteCommand = "DELETE FROM \"Posts\" WHERE \"postID\" = :postID";
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

        public static bool Like(int postId)
        {
            try
            {
                // SQL 语句：将指定 postID 的帖子点赞数加一
                string updateCommand = "UPDATE \"Posts\" SET \"likesCount\" = \"likesCount\" + 1 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }
        }

        public static bool CancleLike(int postId)
        {
            try
            {
                // SQL 语句：将指定 postID 的帖子点赞数加一
                string updateCommand = "UPDATE \"Posts\" SET \"likesCount\" = \"likesCount\" - 1 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }
        }

        public static bool Forward(int postId)
        {
            try
            {
                // SQL 语句：将指定 postID 的帖子转发加一
                string updateCommand = "UPDATE \"Posts\" SET \"forwardCount\" = \"forwardCount\" + 1 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }
        }

        public static bool Report(int postId) {
            try
            {
                string updateCommand = "UPDATE \"Posts\" SET \"isReported\" = 1 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }
        }

        public static bool CancleReport(int postId) {
            try
            {
                string updateCommand = "UPDATE \"Posts\" SET \"isReported\" = 0 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }

        }

        public static bool Pin(int postId)
        {
            try
            {
                string updateCommand = "UPDATE \"Posts\" SET \"isPinned\" = 1 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }
        }

        public static bool CanclePin(int postId)
        {
            try
            {
                string updateCommand = "UPDATE \"Posts\" SET \"isPinned\" = 0 WHERE \"postID\" = :postID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
                };

                int rowsAffected = OracleHelper.ExecuteNonQuery(updateCommand, null, parameters);

                // 如果更新操作影响了行数，返回 true，表示更新成功
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error incrementing likes for post with postID {postId}: {ex.Message}");
                return false;
            }

        }

        public static PostInfo GetPostByPostID(int postId)
        {
            string selectCommand = "SELECT * FROM \"Posts\" WHERE \"postID\" = :postID";

            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter("postID", OracleDbType.Int32) { Value = postId }
            };

            // 执行查询
            DataTable postTable = OracleHelper.ExecuteTable(selectCommand, parameters);
            DataRow row = postTable.Rows[0];
            PostInfo postInfo = new()
            {
                postID = Convert.ToInt32(row["postID"]),
                userID = Convert.ToInt32(row["userID"]),
                postTitle = row["postTitle"].ToString(),
                postContent = row["postContent"].ToString(),
                postCategory = row["postCategory"].ToString(),
                postTime = Convert.ToDateTime(row["postTime"]),
                likesCount = Convert.ToInt32(row["likesCount"]),
                forwardCount = Convert.ToInt32(row["forwardCount"]),
                commentsCount = Convert.ToInt32(row["commentsCount"]),
                userName = row["userName"].ToString(),
                imgUrl = row["imgUrl"].ToString(),
                isPinned = Convert.ToInt32(row["isPinned"]),
                isReported = Convert.ToInt32(row["isReported"])
            };
            

            return postInfo;
        }
    }
}