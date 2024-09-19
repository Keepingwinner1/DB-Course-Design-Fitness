using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fitness.DAL
{
    public class CommentDAL
    {
        public static Comment ToModel(DataRow row)
        {
            return new Comment
            {
                commentID = Convert.ToInt32(row["commentID"]),
                userID = Convert.ToInt32(row["userID"]),
                postID = Convert.ToInt32(row["postID"]),
                parentCommentID = Convert.IsDBNull(row["parentCommentID"]) ? 0 : Convert.ToInt32(row["parentCommentID"]),
                commentTime = Convert.ToDateTime(row["commentTime"]),
                likesCount = Convert.ToInt32(row["likesCount"]),
                Content = Convert.ToString(row["Content"]),
                userName = Convert.ToString(row["userName"])
            };
        }

        public static List<Comment> ToModelList(DataTable table)
        {
            List<Comment> comments = new List<Comment>();
            foreach (DataRow row in table.Rows)
            {
                comments.Add(ToModel(row));
            }
            return comments;
        }

        public static List<Comment> GetCommentByPostID(int postID)
        {
            string query = "SELECT * FROM \"Comment\" WHERE \"postID\" = :postID AND \"parentCommentID\" = -1 ORDER BY \"commentTime\" DESC";
            OracleParameter[] parameters = {
            new OracleParameter("postID",OracleDbType.Int32){ Value = postID }
    };
            DataTable table = OracleHelper.ExecuteTable(query, parameters);
            return ToModelList(table);
        }

        public static List<Comment> GetCommentByCommentID(int commentID)
        {
            string query = "SELECT * FROM \"Comment\" WHERE \"parentCommentID\" = :commentID ORDER BY \"commentTime\" DESC";
            OracleParameter[] parameters = {
            new OracleParameter("commentID" , OracleDbType.Int32 ) {Value = commentID }
        };
            DataTable table = OracleHelper.ExecuteTable(query, parameters);
            if (table.Rows.Count > 0)
            {
                return ToModelList(table);
            }
            return null;
        }

        public static Comment GetByCommentID(int commentID)
        {
            string query = "SELECT * FROM \"Comment\" WHERE \"commentID\" = :commentID";
            OracleParameter[] parameters = {
            new OracleParameter("commentID" , OracleDbType.Int32 ) {Value = commentID }
        };
            DataTable table = OracleHelper.ExecuteTable(query, parameters);
            if (table.Rows.Count > 0)
            {
                return ToModel(table.Rows[0]);
            }
            return null;
        }

        public static int Insert(Comment comment)
        {
            string query = "INSERT INTO \"Comment\" (\"userID\", \"userName\",\"postID\", \"parentCommentID\", \"commentTime\", \"likesCount\", \"Content\")" +
                "VALUES(:userID,:userName, :postID, :parentCommentID, :commentTime, :likesCount, :Content) " +
                "RETURNING \"commentID\" INTO :commentID";

            OracleParameter[] parameters = {
                new OracleParameter("userID", OracleDbType.Int32) { Value = comment.userID },
                new OracleParameter("userName",OracleDbType.NVarchar2){Value = comment.userName},
                new OracleParameter("postID", OracleDbType.Int32) { Value = comment.postID },
                new OracleParameter("parentCommentID", OracleDbType.Int32) { Value = comment.parentCommentID == -1 ? -1 : (object)comment.parentCommentID },
                new OracleParameter("commentTime", OracleDbType.TimeStamp) { Value = comment.commentTime },
                new OracleParameter("likesCount", OracleDbType.Int32) { Value = comment.likesCount },
                new OracleParameter("Content", OracleDbType.NVarchar2) { Value = comment.Content },
                new OracleParameter("commentID", OracleDbType.Int32, ParameterDirection.Output)
             };
            int rowsAffected = OracleHelper.ExecuteNonQuery(query,null, parameters);
            OracleDecimal oracleInt = (OracleDecimal)parameters[7].Value;
            return oracleInt.ToInt32();
        }

        public static bool DeleteComment(int commentID)
        {
            string query = "DELETE FROM \"Comment\" WHERE \"commentID\" = :commentID";
            OracleParameter[] parameters = {
            new OracleParameter("commentID", OracleDbType.Int32) { Value = commentID }};

            int rowsAffected = OracleHelper.ExecuteNonQuery(query, null, parameters);
            return rowsAffected > 0;
        }

        public static bool LikeCountsAddOne(int commentID)
        {
            string cmdText = "UPDATE \"Comment\" SET \"likesCount\" = \"likesCount\" + 1 WHERE \"commentID\" = :commentID";

            OracleParameter[] parameters = {
            new OracleParameter("commentID", OracleDbType.Int32) { Value = commentID }
        };
            return OracleHelper.ExecuteNonQuery(cmdText,null, parameters) > 0;
        }

        public static bool LikeCountsMinusOne(int commentID)
        {
            string cmdText = "UPDATE \"Comment\" SET \"likesCount\" = \"likesCount\" - 1 WHERE \"commentID\" = :commentID";

            OracleParameter[] parameters = {
            new OracleParameter("commentID", OracleDbType.Int32) { Value = commentID }
        };
            return OracleHelper.ExecuteNonQuery(cmdText, null, parameters) > 0;
        }


        public static bool CommentsCountAddOne(int postID)
        {
            string sql = @"UPDATE ""Posts"" 
                   SET ""commentsCount"" = ""commentsCount"" + 1 
                   WHERE ""postID"" = :postID";

            OracleParameter parameter = new OracleParameter("postID", OracleDbType.Int32) { Value = postID };

            int rowsAffected = OracleHelper.ExecuteNonQuery(sql,null, parameter);

            return rowsAffected > 0;
        }

        public static bool CommentsCountMinusOne(int postID)
        {
            string sql = @"UPDATE ""Posts"" 
                   SET ""commentsCount"" = ""commentsCount"" - 1 
                   WHERE ""postID"" = :postID";

            OracleParameter parameter = new OracleParameter("postID", OracleDbType.Int32) { Value = postID };

            int rowsAffected = OracleHelper.ExecuteNonQuery(sql, null, parameter);

            return rowsAffected > 0;
        }


    }
}
