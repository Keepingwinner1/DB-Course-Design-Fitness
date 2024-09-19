using Fitness.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fitness.Models.Manage;

namespace Fitness.DAL
{
    public class UpdateDAL
    {
        //ToModel Insert GetAll 
        // 将DataRow转换为Update对象
        public static Update ToModel(DataRow row)
        {
            return new Update
            {
                coachID = Convert.ToInt32(row["coachID"]),
                classID = Convert.ToInt32(row["classID"]),
                actionType = row["actionType"].ToString(),
                updateTime = Convert.ToDateTime(row["updateTime"]),
                updateContext = row["updateContext"].ToString()
            };
        }

        // 插入新的Update记录
        public static string Insert(Update update)
        {
            try
            {
                string sql = "INSERT INTO \"Update\"(\"coachID\", \"classID\", \"actionType\", \"updateTime\", \"updateContext\") " +
                             "VALUES(:coachID, :classID, :actionType, :updateTime, :updateContext)";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("coachID", OracleDbType.Int32) { Value = update.coachID },
                new OracleParameter("classID", OracleDbType.Int32) { Value = update.classID },
                new OracleParameter("actionType", OracleDbType.Varchar2) { Value = update.actionType },
                new OracleParameter("updateTime", OracleDbType.Date) { Value = update.updateTime },
                new OracleParameter("updateContext", OracleDbType.Varchar2) { Value = update.updateContext }
                };

                OracleHelper.ExecuteNonQuery(sql,null, parameters);
                return "插入成功";
            }
            catch (Exception ex)
            {
                return $"插入失败: {ex.Message}";
            }
        }

        // 获取所有Update记录
        public static List<Update> GetAll()
        {
            List<Update> updates = new List<Update>();
            try
            {
                string sql = "SELECT * FROM \"Update\"";
                DataTable dt = OracleHelper.ExecuteTable(sql);

                foreach (DataRow row in dt.Rows)
                {
                    updates.Add(ToModel(row));
                }

                return updates;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有记录失败: {ex.Message}");
                return null;
            }
        }

    }
}
