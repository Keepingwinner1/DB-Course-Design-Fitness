using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class CourseRequest
    {
        public Course Course { get; set; }
        public List<CourseSchedule> CourseSchedules { get; set; }
    }


    public class ReserveCourseRequest
    {
        public string token { get; set; }
        public int[] classID { get; set; }
        public string payMethod { get; set; }
    }

    public class PayFareRequest {
       public string token{get; set; }
       public int[] bookID { get; set; }
       public string payMethod { get; set; }
    }

    public class feedback {
        public int traineeID { get; set; }
        public string comment { get; set; }
        public int grade { get; set; }
    }

    public class CourseSchedule { 
        public int classID { get; set; }
        public int dayOfWeek { get; set; }
        public string classTime { get; set;}

    }


    public class Course
    {
        // 课程ID
        public int classID { get; set; }

        // 课程类型ID
        public int typeID { get; set; }

        public string courseName { get; set; }

        public int Capacity { get; set; }

        public string courseDescription { get; set; }

        public int coursePrice { get; set; }

        public DateTime courseStartTime { get; set; } 

        public DateTime courseEndTime { get; set; }

        public int courseLastDays { get; set; } 

        public float courseGrade { get; set; } 
        public string coursePhotoUrl { get; set; }
        public string courseVideoUrl { get; set; }
        public string features { get; set; }
    }

    public class BookCourseInfo
    {
        // 课程ID
        public int classID { get; set; }
        public int bookID { get; set; }

        // 课程类型ID
        public int typeID { get; set; }

        public string courseName { get; set; }

        public int coursePrice { get; set; }

        public DateTime courseStartTime { get; set; }

        public DateTime courseEndTime { get; set; }

        public string coursePhotoUrl { get; set; }

        public string payMethod { get; set; }

        public int bookStatus { get; set; }

        public DateTime bookTime { get; set; }

    }

    public class Participate
    {
        // 学员的标识（外键，引用自Trainee表）
        public int traineeID { get; set; }

        // 课程班级的标识（外键，引用自Course表）
        public int classID { get; set; }

        // 课程类型的标识（外键，引用自Course表）
        public int typeID { get; set; }

        // 用户对课程评分
        public int Grade { get; set; } = -1;

        // 用户对课程评价
        public string Evaluate { get; set; } = "null";
    }

    public class  Teaches 
    { 
        public int coachID { get; set;}
        public int classID { get; set;}
        public int typeID { get; set;}  

    }

    public class Advise 
    {
        public int classID { get; set; }
        public int coachID { get; set;}
        public int traineeID { get; set;}  
    }

    public class Book
    {
        // 预订账单的标识
        public int bookID { get; set; } = -1;

        public int classID { get; set; }

        public int traineeID { get; set; }

        // 支付账单的标识
        public int paymentID { get; set; } = -1;

        public string payMethod { get; set; } = "null";

        public int bookStatus { get; set; } = 0;

        public DateTime bookTime { get; set; } = DateTime.Now;
    
    }


    public class Payment
    {
        // 支付账单的标识
        public int paymentID { get; set; }

        // 预订账单的标识
        public int bookID { get; set; }

        public int Amount { get; set; }

        public string payMethod { get; set; }

        public int paymentStatus { get; set; } = -1;

        public DateTime payTime { get; set; }= DateTime.Now;
    }
}
