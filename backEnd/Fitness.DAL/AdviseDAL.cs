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
    public class AdviseDAL
    {
        // 将DataRow转换为Advise对象
        public static Advise ToModel(DataRow row)
        {
            return new Advise
            {
                classID = Convert.ToInt32(row["classID"]),
                coachID = Convert.ToInt32(row["coachID"]),
                traineeID = Convert.ToInt32(row["traineeID"])
            };
        }

        // 插入新的Advise记录
        public static bool Insert(Advise advise,OracleTransaction transaction)
        {
            try
            {
                string sql = "INSERT INTO \"Advise\"(\"classID\", \"coachID\", \"traineeID\") " +
                             "VALUES(:classID, :coachID, :traineeID)";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = advise.classID },
                new OracleParameter("coachID", OracleDbType.Int32) { Value = advise.coachID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = advise.traineeID }
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

        // 删除指定classID和traineeID的Advise记录
        public static bool DeleteByClassIDAndTraineeID(int classID, int traineeID)
        {
            //OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
/*                conn = OracleHelper.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();*/

                string sql = "DELETE FROM \"Advise\" WHERE \"classID\" = :classID AND \"traineeID\" = :traineeID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = traineeID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);
                transaction.Commit();
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
/*            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }*/
        }

    }
}
