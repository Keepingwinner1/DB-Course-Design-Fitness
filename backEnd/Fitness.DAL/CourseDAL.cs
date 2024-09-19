using Fitness.DAL.Core;
using Fitness.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class CourseDAL
    {
        // 将 DataRow 转换为 Course 对象
        public static Course ToModel(DataRow row)
        {
            Course course = new Course
            {
                classID = Convert.ToInt32(row["classID"]),
                typeID = Convert.ToInt32(row["typeID"]),
                courseName = row["courseName"].ToString(),
                Capacity = Convert.ToInt32(row["Capacity"]),
                courseDescription = row["courseDescription"].ToString(),
                coursePrice = Convert.ToInt32(row["coursePrice"]),
                courseStartTime = Convert.ToDateTime(row["courseStartTime"]),
                courseEndTime = Convert.ToDateTime(row["courseEndTime"]),
                courseLastDays = Convert.ToInt32(row["courseLastDays"]),
                courseGrade = Convert.ToSingle(row["courseGrade"]),
                coursePhotoUrl = row["coursePhotoUrl"].ToString(),
                courseVideoUrl = row["courseVideoUrl"].ToString(),
                features = row["features"].ToString(),
            };
            return course;
        }

        // 将 DataTable 转换为 Course 对象列表
        public static List<Course> ToModelList(DataTable dt)
        {
            List<Course> courses = new List<Course>();
            foreach (DataRow row in dt.Rows)
            {
                courses.Add(ToModel(row));
            }
            return courses;
        }

        // 将 DataTable 转换为 BookCourseInfo 对象列表
        public static List<BookCourseInfo> BookCourseInfoToModelList(DataTable dt)
        {
            List<BookCourseInfo> info = new List<BookCourseInfo>();
            foreach (DataRow row in dt.Rows)
            {
                info.Add(new BookCourseInfo
                {
                    classID = Convert.ToInt32(row["classID"]),
                    bookID = Convert.ToInt32(row["bookID"]),
                    typeID = Convert.ToInt32(row["typeID"]),
                    courseName = row["courseName"].ToString(),
                    coursePrice = Convert.ToInt32(row["coursePrice"]),
                    courseStartTime = Convert.ToDateTime(row["courseStartTime"]),
                    courseEndTime = Convert.ToDateTime(row["courseEndTime"]),
                    coursePhotoUrl = row["coursePhotoUrl"].ToString(),
                    payMethod = row["payMethod"].ToString(),
                    bookStatus = Convert.ToInt32(row["bookStatus"]),
                    bookTime = Convert.ToDateTime(row["bookTime"])
                });
            }
            return info;
        }

        // 根据 classID 获取课程
        public static Course GetCourseByClassID(int classID)
        {
            try
            {
                DataTable dt = OracleHelper.ExecuteTable("SELECT * FROM \"Course\" WHERE \"classID\"=:classID",
                   new OracleParameter("classID", OracleDbType.Int32) { Value = classID });

                    return ToModel(dt.Rows[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // 根据 userID 获取所有已预约但未付款的课程
        public static List<BookCourseInfo> GetReservedCourseByUserID(int userID)
        {
            try
            {
                string sql = "SELECT \"classID\", \"bookID\",\"typeID\", \"courseName\", \"coursePrice\", \"courseStartTime\", " +
                            "\"courseEndTime\", \"coursePhotoUrl\", \"payMethod\", \"bookStatus\", \"bookTime\" " +
                            "FROM \"Course\" NATURAL JOIN \"Book\" " +
                            "WHERE \"traineeID\" = :userID AND \"bookStatus\" = 1";
                DataTable dt = OracleHelper.ExecuteTable(sql,new OracleParameter("userID", OracleDbType.Int32) { Value = userID });

                 return BookCourseInfoToModelList(dt);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //根据userID 获取个人所有已付款（即已参与的课程）
        public static List<Course> GetParticipateddCourseByUserID(int userID)
        {
            try
            {
                string sql = "SELECT * FROM \"Course\" NATURAL JOIN \"Participate\" " +
                            "WHERE \"traineeID\" = :userID";
                DataTable dt = OracleHelper.ExecuteTable(sql, new OracleParameter("userID", OracleDbType.Int32) { Value = userID });

                 return ToModelList(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //获取教练本人所教的所有课程
        public static List<Course> GetPublishedCourseByUserID(int userID)
        {
            try
            {
                string sql = "SELECT * FROM \"Course\" NATURAL JOIN \"Teaches\" " +
                             "WHERE \"coachID\" = :userID";
                DataTable dt = OracleHelper.ExecuteTable(sql, new OracleParameter("userID", OracleDbType.Int32) { Value = userID });
                return ToModelList(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        //学员获取今日课程列表
        public static string GetTodayCoursesByUserID(int userID)
        {
            List<Dictionary<string, string>> todayCourses = new List<Dictionary<string, string>>();
            try
            {
                // 获取今天的日期
                DateTime today = DateTime.Now.Date;
                int todayDayOfWeek = (int)today.DayOfWeek; // 星期日为0, 星期一为1, ..., 星期六为6

                // 1. 获取用户今天参与的课程ID和时间
                string participateQuery = "SELECT p.\"classID\", cs.\"classTime\" " +
                                          "FROM \"Participate\" p " +
                                          "JOIN \"CourseSchedule\" cs ON p.\"classID\" = cs.\"classID\" " +
                                          "WHERE p.\"traineeID\" = :userID AND cs.\"dayOfWeek\" = :todayDayOfWeek";

                OracleParameter[] participateParams = new OracleParameter[]
                {
                new OracleParameter("userID", userID),
                new OracleParameter("todayDayOfWeek", todayDayOfWeek)
                };

                DataTable participateDt = OracleHelper.ExecuteTable(participateQuery, participateParams);

                if (participateDt.Rows.Count > 0)
                {
                    // 2. 获取今日的课程安排和课程详细信息
                    foreach (DataRow row in participateDt.Rows)
                    {
                        int classID = Convert.ToInt32(row["classID"]);
                        string classTime = row["classTime"].ToString();

                        // 获取课程详细信息
                        string courseQuery = "SELECT * FROM \"Course\" WHERE \"classID\" = :classID";
                        OracleParameter[] courseParams = new OracleParameter[]
                        {
                        new OracleParameter("classID", classID)
                        };

                        DataTable courseDt = OracleHelper.ExecuteTable(courseQuery, courseParams);

                        if (courseDt.Rows.Count > 0)
                        {
                            foreach (DataRow courseRow in courseDt.Rows)
                            {
                                var courseInfo = new Dictionary<string, string>
                            {
                                { "classID", courseRow["classID"].ToString() },
                                { "courseName", courseRow["courseName"].ToString() },
                                { "courseDescription", courseRow["courseDescription"].ToString() },
                                { "courseStartTime", Convert.ToDateTime(courseRow["courseStartTime"]).ToString("yyyy-MM-dd") },
                                { "courseEndTime", Convert.ToDateTime(courseRow["courseEndTime"]).ToString("yyyy-MM-dd") },
                                { "courseGrade", Convert.ToInt32(courseRow["courseGrade"]).ToString() },
                                { "coursePrice", Convert.ToInt32(courseRow["coursePrice"]).ToString() },
                                { "coursePhotoUrl", courseRow["coursePhotoUrl"].ToString() },
                                { "classTime", classTime }
                            };

                                todayCourses.Add(courseInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户今日课程失败: {ex.Message}");
            }

            return JsonConvert.SerializeObject(todayCourses);
        }

        //教练获取今日课程列表
        public static string GetCoachTodayCoursesByUserID(int userID)
        {
            List<Dictionary<string, string>> todayCourses = new List<Dictionary<string, string>>();
            try
            {
                // 获取今天的日期
                DateTime today = DateTime.Now.Date;
                int todayDayOfWeek = (int)today.DayOfWeek; // 星期日为0, 星期一为1, ..., 星期六为6

                // 1. 获取用户今天参与的课程ID和时间
                string participateQuery = "SELECT p.\"classID\", cs.\"classTime\" " +
                                          "FROM \"Teaches\" p " +
                                          "JOIN \"CourseSchedule\" cs ON p.\"classID\" = cs.\"classID\" " +
                                          "WHERE p.\"coachID\" = :userID AND cs.\"dayOfWeek\" = :todayDayOfWeek";

                OracleParameter[] participateParams = new OracleParameter[]
                {
                new OracleParameter("userID", userID),
                new OracleParameter("todayDayOfWeek", todayDayOfWeek)
                };

                DataTable participateDt = OracleHelper.ExecuteTable(participateQuery, participateParams);

                if (participateDt.Rows.Count > 0)
                {
                    // 2. 获取今日的课程安排和课程详细信息
                    foreach (DataRow row in participateDt.Rows)
                    {
                        int classID = Convert.ToInt32(row["classID"]);
                        string classTime = row["classTime"].ToString();

                        // 获取课程详细信息
                        string courseQuery = "SELECT * FROM \"Course\" WHERE \"classID\" = :classID";
                        OracleParameter[] courseParams = new OracleParameter[]
                        {
                        new OracleParameter("classID", classID)
                        };

                        DataTable courseDt = OracleHelper.ExecuteTable(courseQuery, courseParams);

                        if (courseDt.Rows.Count > 0)
                        {
                            foreach (DataRow courseRow in courseDt.Rows)
                            {
                                var courseInfo = new Dictionary<string, string>
                            {
                                { "classID", courseRow["classID"].ToString() },
                                { "courseName", courseRow["courseName"].ToString() },
                                { "courseDescription", courseRow["courseDescription"].ToString() },
                                { "courseStartTime", Convert.ToDateTime(courseRow["courseStartTime"]).ToString("yyyy-MM-dd") },
                                { "courseEndTime", Convert.ToDateTime(courseRow["courseEndTime"]).ToString("yyyy-MM-dd") },
                                { "courseGrade", Convert.ToInt32(courseRow["courseGrade"]).ToString() },
                                { "coursePrice", Convert.ToInt32(courseRow["coursePrice"]).ToString() },
                                { "coursePhotoUrl", courseRow["coursePhotoUrl"].ToString() },
                                { "classTime", classTime }
                            };

                                todayCourses.Add(courseInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取教练今日课程失败: {ex.Message}");
            }
            return JsonConvert.SerializeObject(todayCourses);
        }

        // 随机获取所有课程
        public static List<Course> GetAllCourseRandomly()
        {
            try
            {
                DataTable dt = OracleHelper.ExecuteTable("SELECT * FROM \"Course\" ORDER BY DBMS_RANDOM.VALUE");
                return ToModelList(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Course>();
            }
        }

        //插入课程
        public static int Insert(Course course, OracleTransaction transaction = null)
        {
            try
            {
                string photoUrl =course.coursePhotoUrl;
                if (photoUrl != "null" && !UrlHelper.IsUrl(photoUrl))
                {
                    string object_name = "course" + course.classID+ "_icon" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                    int base64Index = photoUrl.IndexOf("base64,")+7 ;
                    photoUrl = photoUrl.Substring(base64Index);
                    OSSHelper.UploadBase64ImageToOss(photoUrl, object_name);
                    photoUrl = OSSHelper.GetPublicObjectUrl(object_name);
                }
                //上传视频的方法加入
                string videoUrl = course.courseVideoUrl;

                string sql = $"INSERT INTO \"Course\"( \"typeID\", \"courseName\", \"Capacity\", \"courseDescription\", \"coursePrice\", \"courseStartTime\", \"courseEndTime\", \"courseLastDays\", \"courseGrade\", \"coursePhotoUrl\",\"courseVideoUrl\",\"features\") " +
                             $"VALUES(:typeID, :courseName, :Capacity, :courseDescription, :coursePrice, :courseStartTime, :courseEndTime, :courseLastDays, :courseGrade, :coursePhotoUrl, :courseVideoUrl, :features) " +
                             $"RETURNING \"classID\" INTO :classID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("typeID", OracleDbType.Int32) { Value = course.typeID },
                    new OracleParameter("courseName", OracleDbType. NVarchar2) { Value = course.courseName },
                    new OracleParameter("Capacity", OracleDbType.Int32) { Value = course.Capacity },
                    new OracleParameter("courseDescription", OracleDbType.NVarchar2) { Value = course.courseDescription },
                    new OracleParameter("coursePrice", OracleDbType.Int32) { Value = course.coursePrice },
                    new OracleParameter("courseStartTime", OracleDbType.Date) { Value = course.courseStartTime },
                    new OracleParameter("courseEndTime", OracleDbType.Date) { Value = course.courseEndTime },
                    new OracleParameter("courseLastDays", OracleDbType.Int32) { Value = course.courseLastDays },
                    new OracleParameter("courseGrade", OracleDbType.BinaryFloat) { Value = course.courseGrade },
                    new OracleParameter("coursePhotoUrl", OracleDbType.Varchar2) { Value = photoUrl },
                    new OracleParameter("courseVideoUrl", OracleDbType.Varchar2) { Value = videoUrl },
                    new OracleParameter("features", OracleDbType.NVarchar2) { Value = course.features },
                    new OracleParameter("classID", OracleDbType.Int32, ParameterDirection.Output)
                };

                OracleHelper.ExecuteNonQuery(sql, null, parameters);
                // 读取 classID 参数的值
                var classIDParam = parameters[12];
                int classID = classIDParam.Value is OracleDecimal oracleDecimal
                    ? oracleDecimal.ToInt32()
                    : Convert.ToInt32(classIDParam.Value);

                return classID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"插入课程时出错：{ex.Message}");
                return -1;
            }
        }

        // 根据 classID 删除课程
        public static bool DeleteByClassID(int classID,OracleTransaction transaction)
        {
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID }
                };
                OracleHelper.ExecuteNonQuery("DELETE FROM \"Course\" WHERE \"classID\"=:classID",transaction, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 根据 classID 更新课程
        public static bool UpdateByClassID(Course course)
        {
            try
            {
                string sql = "UPDATE \"Course\" SET \"typeID\"=:typeID, \"courseName\"=:courseName, \"Capacity\"=:Capacity, \"courseDescription\"=:courseDescription, \"coursePrice\"=:coursePrice, \"courseStartTime\"=:courseStartTime, \"courseEndTime\"=:courseEndTime, \"courseLastDays\"=:courseLastDays, \"courseGrade\"=:courseGrade, \"coursePhotoUrl\"=:coursePhotoUrl,\"courseVideoUrl\"=:courseVideoUrl WHERE \"classID\"=:classID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("typeID", OracleDbType.Int32) { Value = course.typeID },
                    new OracleParameter("courseName", OracleDbType.Varchar2) { Value = course.courseName },
                    new OracleParameter("Capacity", OracleDbType.Int32) { Value = course.Capacity },
                    new OracleParameter("courseDescription", OracleDbType.Varchar2) { Value = course.courseDescription },
                    new OracleParameter("coursePrice", OracleDbType.Int32) { Value = course.coursePrice },
                    new OracleParameter("courseStartTime", OracleDbType.Date) { Value = course.courseStartTime },
                    new OracleParameter("courseEndTime", OracleDbType.Date) { Value = course.courseEndTime },
                    new OracleParameter("courseLastDays", OracleDbType.Int32) { Value = course.courseLastDays },
                    new OracleParameter("courseGrade", OracleDbType.BinaryFloat) { Value = course.courseGrade },
                    new OracleParameter("coursePhotoUrl", OracleDbType.Varchar2) { Value = course.coursePhotoUrl },
                    new OracleParameter("courseVideoUrl", OracleDbType.Varchar2) { Value = course.courseVideoUrl },
                    new OracleParameter("classID", OracleDbType.Int32) { Value = course.classID }
                };
                OracleHelper.ExecuteNonQuery(sql,null,parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 根据查询参数进行搜索
        public static List<Course> SearchCourses(CourseSearchParams searchParams)
        {
            List<OracleParameter> parameters = new List<OracleParameter>();
            string query = "SELECT * FROM \"Course\" WHERE 1=1";

            if (!string.IsNullOrEmpty(searchParams.CourseName))
            {
                query += " AND \"courseName\" LIKE :courseName";
                parameters.Add(new OracleParameter(":courseName", "%" + searchParams.CourseName + "%"));
            }

            if (searchParams.TypeID.HasValue)
            {
                query += " AND \"typeID\" = :typeID";
                parameters.Add(new OracleParameter(":typeID", searchParams.TypeID.Value));
            }

            if (searchParams.MinPrice.HasValue)
            {
                query += " AND \"coursePrice\" >= :minPrice";
                parameters.Add(new OracleParameter(":minPrice", searchParams.MinPrice.Value));
            }

            if (searchParams.MaxPrice.HasValue)
            {
                query += " AND \"coursePrice\" <= :maxPrice";
                parameters.Add(new OracleParameter(":maxPrice", searchParams.MaxPrice.Value));
            }
/*
            if (searchParams.StartDate.HasValue)
            {
                query += " AND \"courseStartTime\" >= :startDate";
                parameters.Add(new OracleParameter(":startDate", searchParams.StartDate.Value));
            }

            if (searchParams.EndDate.HasValue)
            {
                query += " AND \"courseEndTime\" <= :endDate";
                parameters.Add(new OracleParameter(":endDate", searchParams.EndDate.Value));
            }*/

            DataTable dt = OracleHelper.ExecuteTable(query,parameters.ToArray());

            return ToModelList(dt);
        }

        public static int GetCoachIDByClassID(int ClassID)
        {
            string selectCommand = "SELECT \"coachID\" FROM \"Teaches\" WHERE \"classID\" = :\"classID\"";
            OracleParameter parameter = new OracleParameter(":classID", ClassID);

            object result = OracleHelper.ExecuteScalar(selectCommand, parameter);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                throw new Exception($"Class not found for ClassID: {ClassID}");
            }
        }

        public static string GetTypeNameByTypeID(int TypeID)
        {
            string query = "SELECT \"typeName\" FROM \"CourseType\" WHERE \"typeID\" = :typeID";
            OracleParameter[] parameters = 
            {
                new OracleParameter(":typeID", OracleDbType.Int32) { Value = TypeID }
            };
            return OracleHelper.ExecuteScalar(query, parameters)?.ToString();
        }
    }
}
