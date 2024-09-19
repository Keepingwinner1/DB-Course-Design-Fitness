using Fitness.BLL;
using Fitness.BLL.Core;
using Fitness.BLL.Interfaces;
using Fitness.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MealRecordsController : ControllerBase
    {
        private readonly IMealRecordBLL _mealRecordBLL;
        private readonly JWTHelper _jwtHelper = new();

        public MealRecordsController(IMealRecordBLL mealRecordBLL)
        {
            _mealRecordBLL = mealRecordBLL;
        }

        // 创建一条饮食记录
        [HttpPost]
        public ActionResult<MealRecordRes> Create(string token, CreateMealRecord createMealRecord)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            Console.WriteLine($"创建一条饮食记录,userID{userID}");
            createMealRecord.mealTime = createMealRecord.mealTime.ToLocalTime();

            MealRecordInfo mealRecordInfo = new()
            {
                userID = userID,
                mealTime = createMealRecord.mealTime,
                mealPhoto = createMealRecord.mealPhoto,
                foods = createMealRecord.foods
            };

            return _mealRecordBLL.Create(mealRecordInfo);
        }


        // 根据recordID获取记录的AI建议
        [HttpGet]
        public ActionResult<AISuggestionRes> AISuggestions(int recordID)
        {
            Console.WriteLine($"获取AI建议:{recordID}");
            return _mealRecordBLL.GetAISuggestion(recordID);
        }

        // 根据userID和日期获取所有饮食记录的详细信息
        [HttpGet]
        public ActionResult<GetAllMealRecordsDetailsRes> GetAllDetails(string token,DateTime date)
        {
            Console.WriteLine($"获取所有饮食记录的详细信息");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            return _mealRecordBLL.GetAllDetails(userID, date);
        }

        // 根据recordID删除某条饮食记录
        [HttpDelete]
        public ActionResult<MessageRes> Delete(int recordID)
        {
            Console.WriteLine($"删除单条饮食记录:{recordID}");
            return _mealRecordBLL.DeleteMealRecord(recordID);
        }

        // 更新饮食记录内容
        [HttpPut]
        public ActionResult<MessageRes> Update(UpdateMealRecordInfo updateMealRecordInfo)
        {
            Console.WriteLine($"更新饮食记录内容");
            updateMealRecordInfo.mealTime = updateMealRecordInfo.mealTime.ToLocalTime();
            return _mealRecordBLL.UpdateMealRecords(updateMealRecordInfo);
        }

        // 根据日期获取饮食记录
        [HttpGet]
        public ActionResult<GetAllMealRecordsNoDetailsRes> GetAllNoDetailsByDate(string token,DateTime date)
        {
            Console.WriteLine($"创建饮食计划");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            return _mealRecordBLL.GetAllNoDetailsByDate(userID, date);
        }

        // AI总结当天饮食记录
        [HttpGet]
        public ActionResult<MessageRes> GetAISummary(string token, DateTime date)
        {
            Console.WriteLine($"AI总结");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            return _mealRecordBLL.MealSummaryByAI(userID, date);
        }
    }
}
