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
    public class TeachesDAL
    {
        // 将 DataRow 转换为 Teaches 对象
        public static Teaches ToModel(DataRow row)
        {
            Teaches teaches = new Teaches
            {
                coachID = Convert.ToInt32(row["coachID"]),
                classID = Convert.ToInt32(row["classID"]),
                typeID = Convert.ToInt32(row["typeID"])
            };
            return teaches;
        }

        // 根据 coachID 和 classID 获取 Teaches 记录
        public static Teaches GetByID(int classID)
        {
            string sql = "SELECT \"coachID\", \"classID\", \"typeID\" " +
                         "FROM \"Teaches\" " +
                         "WHERE \"classID\" = :classID";

            OracleParameter[] parameters = {
            new OracleParameter("classID", OracleDbType.Int32) { Value = classID }
            };

            DataTable dt = OracleHelper.ExecuteTable(sql, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            return ToModel(row);
        }

        // 插入新的 Teaches 记录
        public static bool Insert(Teaches teaches,OracleTransaction transaction)
        {
            try
            {
                string sql = "INSERT INTO \"Teaches\" (\"coachID\", \"classID\", \"typeID\") VALUES (:coachID, :classID, :typeID)";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("coachID", OracleDbType.Int32) { Value = teaches.coachID },
                new OracleParameter("classID", OracleDbType.Int32) { Value = teaches.classID },
                new OracleParameter("typeID", OracleDbType.Int32) { Value = teaches.typeID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction ,parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 根据 coachID 和 classID 删除 Teaches 记录
        public static  bool Delete(int coachID, int classID,OracleTransaction transaction)
        {
            try
            {
                string sql = "DELETE FROM \"Teaches\" WHERE \"coachID\"=:coachID AND \"classID\"=:classID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("coachID", OracleDbType.Int32) { Value = coachID },
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);
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
