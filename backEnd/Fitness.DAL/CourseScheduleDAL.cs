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
    public class CourseScheduleDAL
    {
        // 插入新课程时间表记录
        public static bool InsertCourseSchedule(int courseID, int dayOfWeek, string classTime)
        {
            string query = "INSERT INTO \"CourseSchedule\" (\"classID\", \"dayOfWeek\", \"classTime\") " +
                           "VALUES (:classID, :dayOfWeek, :classTime)";

            OracleParameter[] parameters = new OracleParameter[]
            {
            new OracleParameter("classID", OracleDbType.Int32) { Value = courseID },
            new OracleParameter("dayOfWeek", OracleDbType.Int32) { Value = dayOfWeek },
            new OracleParameter("classTime", OracleDbType.Varchar2) { Value = classTime }
            };

            try
            {
                int rowsAffected = OracleHelper.ExecuteNonQuery(query,null,parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting course schedule: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteCourseSchedulesByClassID(int classID)
        {
            string cmdText = "DELETE FROM \"CourseSchedule\" WHERE \"classID\" = :classID";

            OracleParameter[] parameters = new OracleParameter[]
            {
            new OracleParameter("classID", classID)
            };

            try
            {
                int result = OracleHelper.ExecuteNonQuery(cmdText,null, parameters);
                // 如果受影响的行数大于等于0，则表示删除操作成功
                return result >= 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除操作失败: {ex.Message}");
                return false;
            }
        }

        public static List<CourseSchedule> GetCourseSchedulesByClassID(int classID)
        {
            string cmdText = "SELECT \"classID\", \"dayOfWeek\", \"classTime\" FROM \"CourseSchedule\" WHERE \"classID\" = :classID";

            OracleParameter[] parameters = new OracleParameter[]
            {
            new OracleParameter("classID", classID)
            };

            List<CourseSchedule> courseSchedules = new List<CourseSchedule>();

            try
            {
                DataTable dt = OracleHelper.ExecuteTable(cmdText, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    CourseSchedule courseSchedule = new CourseSchedule
                    {
                        classID = Convert.ToInt32(row["classID"]),
                        dayOfWeek = Convert.ToInt32(row["dayOfWeek"]),
                        classTime = row["classTime"].ToString()
                    };

                    courseSchedules.Add(courseSchedule);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时间表失败: {ex.Message}");
            }

            return courseSchedules;
        }

    }
}
