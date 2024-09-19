using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fitness.DAL
{
    public class ParticipateDAL
    {
        // 将DataRow转换为Participate对象
        public static Participate ToModel(DataRow row)
        {
            return new Participate
            {
                traineeID = Convert.ToInt32(row["traineeID"]),
                classID = Convert.ToInt32(row["classID"]),
                typeID = Convert.ToInt32(row["typeID"]),
                Grade = Convert.ToInt32(row["Grade"]),
                Evaluate = row["Evaluate"].ToString()
            };
        }

        // 插入新的Participate记录
        public static bool Insert(Participate participate,OracleTransaction transaction)
        {
            try
            {
                string sql = "INSERT INTO \"Participate\"(\"traineeID\", \"classID\", \"typeID\", \"Grade\", \"Evaluate\") " +
                             "VALUES(:traineeID, :classID, :typeID, :Grade, :Evaluate)";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = participate.traineeID },
                new OracleParameter("classID", OracleDbType.Int32) { Value = participate.classID },
                new OracleParameter("typeID", OracleDbType.Int32) { Value = participate.typeID },
                new OracleParameter("Grade", OracleDbType.Int32) { Value = participate.Grade },
                new OracleParameter("Evaluate", OracleDbType.Varchar2) { Value = participate.Evaluate }
                };

                OracleHelper.ExecuteNonQuery(sql,transaction,parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"插入失败: {ex.Message}");
                return false;
            }
        }

        // 删除指定classID和traineeID的Participate记录
        public static bool DeleteByClassIDAndTraineeID(int classID, int traineeID)
        {
            OracleTransaction transaction = null;

            try
            {
                string sql = "DELETE FROM \"Participate\" WHERE \"classID\" = :classID AND \"traineeID\" = :traineeID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = traineeID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine($"删除失败: {ex.Message}");
                return false;
            }
        }

        // 根据课程ID获取评论
        public static List<feedback> GetCommentsByClassID(int classID)
        {
            string query = "SELECT \"traineeID\",\"Evaluate\",\"Grade\" FROM \"Participate\" WHERE \"classID\" = :classID";
            OracleParameter parameters = new OracleParameter("classID", OracleDbType.Int32) { Value = classID };

            DataTable dt = OracleHelper.ExecuteTable(query,parameters);
            List<feedback> comments = new List<feedback>();

            foreach (DataRow row in dt.Rows)
            {
                comments.Add(new feedback()
                {
                    traineeID = Convert.ToInt32(row["traineeID"]),
                    comment = Convert.ToString(row["Evaluate"]),
                    grade = Convert.ToInt32(row["Grade"])
                });
            }
            return comments;
        }

        public static List<int> GetGradesByClassID(int classID)
        {
            string query = "SELECT \"Grade\" FROM \"Participate\" WHERE \"classID\" = :classID";
            OracleParameter parameters = new OracleParameter("classID", OracleDbType.Int32) { Value = classID };

            DataTable dt = OracleHelper.ExecuteTable(query, parameters);
            List<int> grades = new List<int>();
            foreach (DataRow row in dt.Rows)
            {
                grades.Add(Convert.ToInt32(row["Grade"]));
            }
            return grades;
        }

        // 根据 classID 和 traineeID 获取参与记录
        public static Participate GetByClassIDAndTraineeID(int classID, int traineeID)
        {
            string query = "SELECT * FROM \"Participate\" WHERE \"classID\" = :classID AND \"traineeID\" = :traineeID";
            OracleParameter[] parameters = {
            new OracleParameter(":classID", classID),
            new OracleParameter(":traineeID", traineeID)
        };

            DataTable dt = OracleHelper.ExecuteTable(query,parameters);

            if (dt.Rows.Count > 0)
            {
                return ToModel(dt.Rows[0]);
            }

            return null;
        }

        // 更新参与记录
        public static int Update(Participate participate)
        {
            string query = "UPDATE \"Participate\" SET \"typeID\" = :typeID,\"Grade\" = :Grade,\"Evaluate\" = :Evaluate WHERE \"classID\" = :classID AND \"traineeID\" = :traineeID";
            OracleParameter[] parameters = {
            new OracleParameter(":typeID", participate.typeID),
            new OracleParameter(":Grade", participate.Grade),
            new OracleParameter(":Evaluate", participate.Evaluate),
            new OracleParameter(":classID", participate.classID),
            new OracleParameter(":traineeID", participate.traineeID)
        };
            return OracleHelper.ExecuteNonQuery(query,null, parameters);
        }
    }
}
