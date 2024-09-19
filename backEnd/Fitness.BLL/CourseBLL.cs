using Fitness.BLL.Core;
using Fitness.DAL;
using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Fitness.DAL.Core;
using System.Data;
using System.Xml.Linq;
using Fitness.BLL.Interfaces;
using Newtonsoft.Json;
using System.Web;

namespace Fitness.BLL
{
    public class CourseBLL : ICourseBLL
    {

        private readonly JWTHelper _jwtHelper = new();
        
        // 发布课程
        public string PublishCourse(string token,Course course,List<CourseSchedule> courseSchedules)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (tokenRes.Role != "coach")
            {
                return "身份权限不符！";
            }

            OracleTransaction transaction = null;
            try
            {
                // 插入课程信息
                int res = CourseDAL.Insert(course);
                if (res == -1)
                {
                    //transaction.Rollback();
                    return "课程发布失败：无法插入课程信息";
                }
                Console.WriteLine(res);
                // 插入教练授课信息
                Teaches teaches = new Teaches
                {
                    coachID = tokenRes.userID,
                    classID = res,
                    typeID = course.typeID
                };

                if (!TeachesDAL.Insert(teaches, transaction))
                {
                    return "课程发布失败：无法插入教练授课信息";
                }

                //插入授课时间段信息
                bool allInserted = true;
                foreach (var schedule in courseSchedules)
                {
                    bool result = CourseScheduleDAL.InsertCourseSchedule(res, schedule.dayOfWeek, schedule.classTime);
                    if (!result)
                    {
                        allInserted = false;
                        // 可根据需求在这里决定是否要中断，或者记录失败的记录
                        return "课程发布失败: 无法插入授课时间信息";
                    }
                }
                return "课程发布成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发布课程时出错：{ex.Message}");
                return $"课程发布失败：{ex.Message}";
            }
        }

        // 修改课程
        public string ModifyCourse(string token,Course course,List<CourseSchedule> courseSchedules)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            if (tokenRes.Role != "coach")
            {
                return "身份权限不符！";
            }

            try
            {
                if (!CourseDAL.UpdateByClassID(course))
                {
                    return "课程修改失败";
                }
                //修改时间段
                // 删除所有与classID相关的课程时间表
                bool deleteResult = CourseScheduleDAL.DeleteCourseSchedulesByClassID(course.classID);
                if (!deleteResult)
                {
                    return $"删除课程时间表失败";
                }
                // 插入新的课程时间表
                bool allInserted = true;
                foreach (var schedule in courseSchedules)
                {
                    bool insertResult =CourseScheduleDAL.InsertCourseSchedule(course.classID, schedule.dayOfWeek, schedule.classTime);
                    if (!insertResult)
                    {
                        allInserted = false;
                        return $"插入课程时间表失败" ;
                    }
                }
                return "课程修改成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"修改课程时出错：{ex.Message}");
                return $"课程修改失败：{ex.Message}";
            }
        }

        // 根据classID删除课程
        public string DeleteCourseByClassID(string token,int classID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);

/*            if (tokenRes.Role != "admin" && tokenRes.Role != "coach")
            {
                return "身份权限不符！";
            }*/

                OracleTransaction transaction = null;
                try
                {
                    if (!CourseDAL.DeleteByClassID(classID, transaction))
                    {
                        return "课程删除失败：无法删除课程信息";
                    }
                //删除授课信息 删除时间段信息
                TeachesDAL.Delete(tokenRes.userID, classID, null);
                CourseScheduleDAL.DeleteCourseSchedulesByClassID(classID);

                    return "课程删除成功";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"删除课程时出错：{ex.Message}");
                    return $"课程删除失败：{ex.Message}";
                }
        }

        // 根据classID获取课程
        public Course GetCourseByClassID(string token, int classID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            try
            {
                return CourseDAL.GetCourseByClassID(classID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时出错：{ex.Message}");
                return null;
            }
        }

        //获取某一个课程的所有参与学员信息
        public List<Trainee> GetAllTraineesByClassID(string token ,int classID) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            try {
                List<Trainee> trainees = new List<Trainee>();
                trainees = TraineeDAL.GetAllTraineesByClassID(classID);
                return trainees;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // 获取所有课程
        public string GetAllCourse(string token)
        {
            try
            {
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                int userID = tokenRes.userID;
                List<Dictionary<string, object>> courseDetails = new List<Dictionary<string, object>>();
                List<Course> courses = CourseDAL.GetAllCourseRandomly();

                foreach (var course in courses)
                {
                    string typeName = CourseDAL.GetTypeNameByTypeID(course.typeID);
                    int coachID = CourseDAL.GetCoachIDByClassID(course.classID);
                    int st;
                    var user = UserDAL.GetUserByUserID(userID, out st);
                    string coachName = user.userName;
                    string instructorHonors = user.Introduction;
                    string iconURL = user.iconURL;

                    var schedule = CourseScheduleDAL.GetCourseSchedulesByClassID(course.classID);

                    int isBooked = 0;
                    //无论是已预订还是已支付，都返回1，代表课程大厅不需要再显示加入购物车按钮
                    if(BookDAL.IsBooked(course.classID,userID))
                        isBooked = 1;
                    Console.WriteLine(isBooked);

                    var courseInfo = new Dictionary<string, object>
                    {
                        { "coursePhotoUrl", course.coursePhotoUrl },
                        { "courseName", course.courseName },
                        { "courseDescription", course.courseDescription },
                        { "courseStartTime", course.courseStartTime.ToString("yyyy-MM-dd") },
                        { "courseEndTime", course.courseEndTime.ToString("yyyy-MM-dd") },
                        { "courseGrade", course.courseGrade.ToString() },
                        { "coursePrice", course.coursePrice },
                        { "coachName", coachName },
                        { "instructorHonors", instructorHonors },
                        { "iconURL", iconURL },
                        { "features", course.features },
                        { "courseType", typeName },
                        { "capacity",course.Capacity},
                        { "isBooked",isBooked},
                        { "schedules", schedule } // Add the schedule information here
                    };

                    courseDetails.Add(courseInfo);
                }

                return JsonConvert.SerializeObject(courseDetails, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有课程时出错：{ex.Message}");
                return "";
            }
        }


        public string BeTrainee(string token,Trainee trainee) 
        {
            try
            {
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                string role = UserDAL.GetRole(tokenRes.userID);
                if (role == "trainee" ) {
                    return "您已是学员身份！";
                }

                bool result = TraineeDAL.Insert(trainee);
                UserDAL.SetRole(tokenRes.userID, "trainee");
                if (result)
                {
                    return "成功转变为学员身份！";
                }
                else
                {
                    return $"转变学员失败: {result}";
                }
            }
            catch (Exception ex)
            {
                return $"转变学员失败: {ex.Message}";
            }
        }

        // 预订课程
        public string ReserveCourse(string token, int[] classIDs, string payMethod)
        {
            var bookIDs = new List<int>(); // 用于存储每次预订的 bookID
            string message = "课程预订成功"; // 默认成功消息

            try
            {
                // 验证 Token
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);

                // 遍历每个 classID 进行预订
                foreach (int classID in classIDs)
                {
                    // 预订课程
                    int result = BookDAL.Insert(new Book
                    {
                        bookID = -1,
                        classID = classID,
                        traineeID = tokenRes.userID,
                        payMethod = payMethod,
                        bookStatus = 1
                    });

                    // 将 bookID 添加到列表中
                    bookIDs.Add(result);

                    // 如果有任何预订失败，则更新消息
                    if (result == -1)
                    {
                        message = "部分课程预订失败";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // 如果出现异常，则为所有 classID 返回失败的 bookID (-1) 和异常消息
                return JsonConvert.SerializeObject(new
                {
                    bookIDs = classIDs.Select(id => -1).ToArray(), // 用 -1 表示所有预订失败
                    message = "课程预订失败，发生异常"
                });
            }

            // 返回 bookID 列表和消息的 JSON 字符串
            return JsonConvert.SerializeObject(new
            {
                bookIDs = bookIDs.ToArray(), // 转换为数组以保证一致性
                message = message
            });
        }

        // 支付课程费用
        public string PayCourseFare(string token, int[] bookIDs,string payMethod)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int traineeID = tokenRes.userID;
            OracleTransaction transaction = null;
            try
            {
                // 检查学员是否已经在Trainee表中
                int st;
                User userTrainee = UserDAL.GetUserByUserID(traineeID, out st);
                Trainee trainee = new Trainee(traineeID, userTrainee.userName, userTrainee.Age, userTrainee.Gender, userTrainee.iconURL, userTrainee.userName);

                string traineeCheckResult = BeTrainee(token, trainee);
                if (traineeCheckResult != "成功转变为学员身份！" && traineeCheckResult != "您已是学员身份！")
                {
                    return traineeCheckResult; // 如果插入Trainee表失败，返回错误信息
                }

                // 遍历每个 bookID 进行支付处理
                foreach (int bookID in bookIDs)
                {
                    var course = CourseDAL.GetCourseByClassID(BookDAL.GetBookByID(bookID).classID);
                    //获取每一个课程的费用
                    int amount=course.coursePrice;
                    string courseName = course.courseName;
                    int fare = -amount;
                    // 调用支付接口
                    var vigorTokenBLL = new VigorTokenBLL();
                    vigorTokenBLL.UpdateBalance(traineeID, $"购买课程:{courseName}, 耗费 {amount} 活力币", fare);
                    // 更新 Payment 表
                    int paymentResultID = PaymentDAL.Insert(new Payment
                    {
                        bookID = bookID,
                        Amount = amount,
                        payMethod = payMethod
                    }, transaction);

                    if (paymentResultID == -1)
                    {
                        return $"支付课程费用失败: 预订账单号 {bookID} 更新 Payment 表异常！";
                    }

                    int bookstatus = 2;//3代表买后已取消，2代表已支付，1代表已预订，0代表预订后已取消/初始状态
                    // 更新 Book 表的支付状态和支付ID
                    bool bookResult = BookDAL.UpdateBookStatusAndPaymentID(bookID, bookstatus,paymentResultID);
                    if (!bookResult)
                    {
                        return $"支付课程费用失败：预订账单号 {bookID} 更新 Book 表异常！";
                    }

                    // 向 Advise 表中插入一条信息
                    bool adviseResult = AdviseDAL.Insert(new Advise
                    {
                        classID = BookDAL.GetBookByID(bookID).classID,
                        coachID = TeachesDAL.GetByID(BookDAL.GetBookByID(bookID).classID).coachID,
                        traineeID = traineeID
                    }, transaction);

                    if (!adviseResult)
                    {
                        return $"支付课程费用失败: 预订账单号 {bookID} 更新 Advise 表异常！";
                    }

                    // 向 Participate 表中插入一条信息
                    bool participateResult = ParticipateDAL.Insert(new Participate
                    {
                        classID = BookDAL.GetBookByID(bookID).classID,
                        traineeID = traineeID,
                        typeID = CourseDAL.GetCourseByClassID(BookDAL.GetBookByID(bookID).classID).typeID
                    }, transaction);

                    if (!participateResult)
                    {
                        return $"支付课程费用失败: 预订账单号 {bookID} 更新 Participate 表异常！";
                    }
                }

                return "支付课程费用成功!";
            }
            catch (Exception ex)
            {
                return $"支付失败: {ex.Message}";
            }
        }

        public string CancelBook(string token, int bookID) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            // 更改Book记录
            bool bookResult = BookDAL.UpdateBookStatus(bookID, 0);
            if (!bookResult)
            {
                return "取消课程预订失败：更改Book记录异常！";
            }
            return "取消预订成功！";
        }

        // 取消课程预订
        public string CancelCourse(string token,int classID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            OracleConnection conn = null;
            OracleTransaction transaction = null;
            try
            {
                int bookID = BookDAL.GetBookByClassIDAndUserID(classID, tokenRes.userID).bookID;
                // 删除Participate记录
                bool participateResult = ParticipateDAL.DeleteByClassIDAndTraineeID(classID, tokenRes.userID);
                if (!participateResult)
                {
                    return "取消课程预订失败：删除Participate记录异常！";
                }

                int amount = PaymentDAL.GetByBookID(bookID).Amount;
                int fare = amount;

                //！！！调用退款接口
                var vigorTokenBLL = new VigorTokenBLL();
                vigorTokenBLL.UpdateBalance(tokenRes.userID, $"退出课程，预订账单号{bookID},退还{amount}活力币", fare);

                // 更改Book记录
                bool bookResult = BookDAL.UpdateBookStatus(bookID,3);
                if (!bookResult)
                {
                    return "取消课程预订失败：更改Book记录异常！";
                }
                return "取消课程成功！";
            }
            catch (Exception ex)
            {
                return $"取消失败: {ex.Message}";
            }
        }

        // 根据userID获取课程
        public string GetReservedCourseByUserID(string token)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID =tokenRes.userID;
            try
            {
                List<Dictionary<string, object>> courseDetails = new List<Dictionary<string, object>>();
                var courses = CourseDAL.GetReservedCourseByUserID(userID);
                foreach (var course in courses)
                {
                    //var schedules = CourseDAL.GetCourseByClassID()
                    var schedule = CourseScheduleDAL.GetCourseSchedulesByClassID(course.classID);
                    /*classID ,bookID,typeID ,courseName ,coursePrice ,courseStartTime,courseEndTime ,coursePhotoUrl,payMethod,bookStatus, bookTime*/
                    var courseInfo = new Dictionary<string, object>
                    {
                        { "coursePhotoUrl", course.coursePhotoUrl },
                        { "courseName", course.courseName },
                        { "courseStartTime", course.courseStartTime.ToString("yyyy-MM-dd") },
                        { "courseEndTime", course.courseEndTime.ToString("yyyy-MM-dd") },
                        { "coursePrice", course.coursePrice},
                        { "classID", course.classID },
                        { "bookID", course.bookID },
                        { "schedules", schedule }
                    };
                    courseDetails.Add(courseInfo);

                }

                return JsonConvert.SerializeObject(courseDetails, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时出错：{ex.Message}");
                return null;
            }
        }

        public string GetParticipatedCourseByUserID(string token) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            try
            {
                List<Dictionary<string, object>> courseDetails = new List<Dictionary<string, object>>();
                List<Course> courses = CourseDAL.GetParticipateddCourseByUserID(userID);
                foreach (Course course in courses)
                {
                    string typeName = CourseDAL.GetTypeNameByTypeID(course.typeID);
                    int coachID = CourseDAL.GetCoachIDByClassID(course.classID);
                    int st;
                    var user = UserDAL.GetUserByUserID(userID, out st);
                    string coachName = user.userName;
                    string instructorHonors = user.Introduction;
                    string iconURL = user.iconURL;
                    var schedule = CourseScheduleDAL.GetCourseSchedulesByClassID(course.classID);

                    var courseInfo = new Dictionary<string, object>
                    {
                        { "coursePhotoUrl", course.coursePhotoUrl },
                        { "courseName", course.courseName },
                        { "courseDescription", course.courseDescription },
                        { "courseStartTime", course.courseStartTime.ToString("yyyy-MM-dd") },
                        { "courseEndTime", course.courseEndTime.ToString("yyyy-MM-dd") },
                        { "courseGrade", course.courseGrade },
                        { "coursePrice", course.coursePrice },
                        { "coachName", coachName },
                        { "capacity",course.Capacity},
                        { "instructorHonors", instructorHonors },
                        { "iconURL", iconURL },
                        { "features", course.features },
                        { "courseType", typeName },
                        { "schedules", schedule }
                    };
                    courseDetails.Add(courseInfo);

                }

                return JsonConvert.SerializeObject(courseDetails, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时出错：{ex.Message}");
                return null;
            }
        }
        public string GetPublishedCourseByUserID(string token) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            try
            {
                List<Dictionary<string, object>> courseDetails = new List<Dictionary<string, object>>();
                List<Course> courses = CourseDAL.GetPublishedCourseByUserID(userID);
                foreach (Course course in courses)
                {
                    string typeName = CourseDAL.GetTypeNameByTypeID(course.typeID);                    
                    int coachID = CourseDAL.GetCoachIDByClassID(course.classID);
                    int st;
                    var user = UserDAL.GetUserByUserID(userID, out st);
                    string coachName = user.userName;
                    string instructorHonors = user.Introduction;
                    string iconURL = user.iconURL;
                    var schedule = CourseScheduleDAL.GetCourseSchedulesByClassID(course.classID);

                    var courseInfo = new Dictionary<string, object>
                    {
                        { "coursePhotoUrl", course.coursePhotoUrl },
                        { "courseName", course.courseName },
                        { "courseDescription", course.courseDescription },
                        { "courseStartTime", course.courseStartTime.ToString("yyyy-MM-dd") },
                        { "courseEndTime", course.courseEndTime.ToString("yyyy-MM-dd") },
                        { "courseGrade", course.courseGrade },
                        { "coursePrice", course.coursePrice },
                        { "coachName", coachName },
                        { "instructorHonors", instructorHonors },
                        { "capacity",course.Capacity},
                        { "iconURL", iconURL },
                        { "features", course.features },
                        { "courseType", typeName },
                        { "schedules", schedule } 
                    };
                    courseDetails.Add(courseInfo);

                }

                return JsonConvert.SerializeObject(courseDetails, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时出错：{ex.Message}");
                return null;
            }
        }


        public string GetTodayCoursesByUserID(string token) {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            try
            {
                return CourseDAL.GetTodayCoursesByUserID(userID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时出错：{ex.Message}");
                return "获取课程时出错";
            }
        }

        public string GetCoachTodayCoursesByUserID(string token)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            try
            {
                return CourseDAL.GetCoachTodayCoursesByUserID(userID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取课程时出错：{ex.Message}");
                return "获取课程时出错";
            }
        }

        // 搜索课程
        public List<Course> SearchCourse(string token,string keyword, int typeID = -1, int minPrice = 0, int maxPrice = int.MaxValue)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            // 检查关键字是否为空
            if (string.IsNullOrEmpty(keyword) && typeID == -1 && minPrice == 0 && maxPrice == int.MaxValue)
            {
                throw new ArgumentException("至少需要一个搜索条件");
            }

            // 调用 CourseDAL 的搜索方法
            return CourseDAL.SearchCourses(new CourseSearchParams {
                   CourseName = keyword,
                   TypeID = typeID,
                   MinPrice = minPrice,
                   MaxPrice = maxPrice
                    });
        }

        // 评分课程
        public string GradeCourse(string token,int classID, int grade)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int traineeID = tokenRes.userID;
            // 获取参与记录
            Participate participate = ParticipateDAL.GetByClassIDAndTraineeID(classID, traineeID);

            if (participate == null)
            {
                return "未找到该课程的参与记录";
            }

            // 更新评分
            participate.Grade = grade;
            try
            {
                if (ParticipateDAL.Update(participate) > 0)
                {
                    return "评分成功";
                }
                else
                {
                    return "评分失败";
                }
            }
            catch (Exception ex)
            {
                return $"评分过程中发生错误: {ex.Message}";
            }
        }

        // 发布评论
        public string PublishComment(string token,int classID, string comment)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int traineeID = tokenRes.userID;

            // 获取参与记录
            Participate participate = ParticipateDAL.GetByClassIDAndTraineeID(classID, traineeID);

            if (participate == null)
            {
                return "未找到该课程的参与记录";
            }

            // 更新评论
            participate.Evaluate = comment;

            try
            {
                if (ParticipateDAL.Update(participate) > 0)
                {
                    return "评论发布成功";
                }
                else
                {
                    return "评论发布失败";
                }
            }
            catch (Exception ex)
            {
                return $"评论发布过程中发生错误: {ex.Message}";
            }
        }

        // 根据课程ID获取评论+评分
        public string GetCourseCommentByClassID(string token,int classID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            try
            {
                List<feedback> comments = ParticipateDAL.GetCommentsByClassID(classID);
                List<Dictionary<string, object>> courseDetails = new List<Dictionary<string, object>>();
                foreach (feedback feedback in comments)
                {
                    int st;
                    User user = UserDAL.GetUserByUserID(feedback.traineeID, out st);
                    var courseInfo = new Dictionary<string, object>
                    {
                        { "avatar", user.iconURL},
                        { "nickname", user.userName },
                        { "content", feedback.comment},
                        { "rating", feedback.grade}
                    };
                    courseDetails.Add(courseInfo);
                }
                return JsonConvert.SerializeObject(courseDetails, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取评论失败: {ex.Message}");
                throw;
            }
        }

        //获取评分
        public double GetAverageGrade(string token, int classID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            try
            {
                List<int> grades = ParticipateDAL.GetGradesByClassID(classID);
                //计算平均分
                double grade = grades.Average();
                return grade;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取评分失败: {ex.Message}");
                throw;
            }
        }

    }
}
