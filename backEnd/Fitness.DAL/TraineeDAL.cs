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
    public class TraineeDAL
    {
        // 将 DataRow 转换为 Trainee 对象
        public static Trainee ToModel(DataRow row)
        {
            Trainee trainee = new Trainee
             (
                Convert.ToInt32(row["traineeID"]),
                 row["userName"].ToString(),
                Convert.ToInt32(row["Age"]),
                row["Gender"].ToString(),
                row["iconURL"].ToString(),
                row["traineeName"].ToString()
            );
            return trainee;
        }

        // 将 DataTable 转换为 Trainee 对象列表
        public static List<Trainee> ToModelList(DataTable dt)
        {
            List<Trainee> trainees = new List<Trainee>();
            foreach (DataRow row in dt.Rows)
            {
                trainees.Add(ToModel(row));
            }
            return trainees;
        }

        public static List<Trainee> GetAllTraineesByClassID(int classID) {
            try{
                string sql = "SELECT \"traineeID\",\"userName\",\"Age\",\"Gender\",\"iconURL\",\"traineeName\" FROM \"Trainee\" NATURAL JOIN \"Participate\" WHERE \"classID\" = :classID";
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID },
                };
                var dt=OracleHelper.ExecuteTable(sql, parameters);
                return ToModelList(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        
        }

        // 插入新的 Trainee 记录
        public static bool Insert(Trainee trainee)
        {
            try
            {
                string sql = "INSERT INTO \"Trainee\" ( \"traineeID\",\"userName\", \"Age\", \"Gender\", \"iconURL\", \"traineeName\") VALUES (:traineeID,:userName, :Age, :Gender, :iconURL, :traineeName)";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = trainee.traineeID },
                new OracleParameter("userName", OracleDbType.Varchar2) { Value = trainee.userName },
                new OracleParameter("Age", OracleDbType.Int32) { Value = trainee.Age },
                new OracleParameter("Gender", OracleDbType.Varchar2) { Value = trainee.Gender },
                new OracleParameter("iconURL", OracleDbType.Varchar2) { Value = trainee.iconURL },
                new OracleParameter("traineeName", OracleDbType.Varchar2) { Value = trainee.traineeName }
                };
                OracleHelper.ExecuteNonQuery(sql,null, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 删除指定classID和traineeID的Trainee记录
        public static bool DeleteByClassIDAndTraineeID(int classID, int traineeID)
        {
            //OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
/*                conn = OracleHelper.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();
*/
                string sql = "DELETE FROM \"Participate\" WHERE \"classID\" = :classID AND \"traineeID\" = :traineeID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = traineeID }
                };

                // 执行删除操作
                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);
                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
/*                if (transaction != null)
                {
                    transaction.Rollback();
                }*/
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
