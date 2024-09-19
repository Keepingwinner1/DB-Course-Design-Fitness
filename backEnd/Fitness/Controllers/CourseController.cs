using Fitness.BLL;
using Fitness.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseBLL _courseBLL;

        public CourseController()
        {
            _courseBLL = new CourseBLL();
        }

        [HttpPost]
        public IActionResult PublishCourse(string token, [FromBody] CourseRequest request)
        {
            if (string.IsNullOrEmpty(token) || request == null || request.Course == null || request.CourseSchedules == null)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                string result = _courseBLL.PublishCourse(token, request.Course, request.CourseSchedules);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing course: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult ModifyCourse(string token, [FromBody] CourseRequest request)
        {
            if (string.IsNullOrEmpty(token) || request == null || request.Course == null || request.CourseSchedules == null)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                string result = _courseBLL.ModifyCourse(token, request.Course, request.CourseSchedules);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error modifying course: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete]
        public ActionResult<string> DeleteCourseByClassID(string token, int classID)
        {
            return _courseBLL.DeleteCourseByClassID(token, classID);
        }

        [HttpGet]
        public ActionResult<Course> GetCourseByClassID(string token, int classID)
        {
            return _courseBLL.GetCourseByClassID(token, classID);
        }

        [HttpGet]
        public ActionResult<string> GetAllCourse(string token)
        {
            return _courseBLL.GetAllCourse(token);
        }

        [HttpGet]
        public ActionResult<List<Trainee>> GetAllTraineesByClassID(string token, int classID) {
            return _courseBLL.GetAllTraineesByClassID(token, classID);
        }

        [HttpPost]
        public ActionResult<string> ReserveCourse([FromBody] ReserveCourseRequest request)
        {
            return _courseBLL.ReserveCourse(request.token, request.classID, request.payMethod);
        }

        [HttpPost]
        public ActionResult<string> PayCourseFare([FromBody] PayFareRequest request)
        {
            return _courseBLL.PayCourseFare(request.token, request.bookID, request.payMethod);
        }

        [HttpPost]
        public ActionResult<string> CancelCourse(string token, int classID)
        {
            return _courseBLL.CancelCourse(token, classID);
        }

        [HttpPost]
        public ActionResult<string> CancelBook(string token, int bookID) {
            return _courseBLL.CancelBook(token, bookID);
         }

        [HttpGet]
        public ActionResult<string> GetReservedCourseByUserID(string token)
        {
            return _courseBLL.GetReservedCourseByUserID(token);
        }
        
        [HttpGet]
        public ActionResult<string> GetParticipatedCourseByUserID(string token)
        {
            return _courseBLL.GetParticipatedCourseByUserID(token);
        }
        
        [HttpGet]
        public ActionResult<string> GetCoachParticipatedCourseByUserID(string token)
        {
            return _courseBLL.GetPublishedCourseByUserID(token);
        }

        [HttpGet]
        public ActionResult<string> GetTodayCoursesByUserID(string token)
        {
            return _courseBLL.GetTodayCoursesByUserID(token);
        }

        [HttpGet]
        public ActionResult<string> GetCoachTodayCoursesByUserID(string token)
        {
            return _courseBLL.GetCoachTodayCoursesByUserID(token);
        }

        [HttpGet]
        public ActionResult<List<Course>> SearchCourse(string token, [FromQuery] string keyword, [FromQuery] int typeID = -1, [FromQuery] int minPrice = 0, [FromQuery] int maxPrice = int.MaxValue)
        {
            return _courseBLL.SearchCourse(token, keyword, typeID, minPrice, maxPrice);
        }

        [HttpPost]
        public ActionResult<string> GradeCourse(string token, int classID, int grade)
        {
            return _courseBLL.GradeCourse(token, classID, grade);
        }

        [HttpPost]
        public ActionResult<string> PublishComment(string token, int classID, string comment)
        {
            return _courseBLL.PublishComment(token, classID, comment);
        }

        [HttpGet]
        public ActionResult<string> GetCourseCommentByClassID(string token, int classID)
        {
            return _courseBLL.GetCourseCommentByClassID(token, classID);
        }

        [HttpGet]
        public ActionResult<double> GetAverageGrade(string token, int classID) { 
            return _courseBLL.GetAverageGrade(token, classID);
        }
    }
}
