using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IMealRecordBLL
    {
        // 创建一条饮食记录
        public MealRecordRes Create(MealRecordInfo mealRecordInfo);

        // 根据日期获取所有饮食记录
        public GetAllMealRecordsDetailsRes GetAllDetails(int userID,DateTime date);

        // 删除饮食记录
        public MessageRes DeleteMealRecord(int recordID);

        // 更新饮食记录
        public MessageRes UpdateMealRecords(UpdateMealRecordInfo updateMealRecordInfo);

        // 根据日期获取所有饮食记录的缩略信息
        public GetAllMealRecordsNoDetailsRes GetAllNoDetailsByDate(int userID, DateTime date);

        // 获取饮食记录的AI分析
        public AISuggestionRes GetAISuggestion(int recordID);

        // 获取AI对饮食记录的当日总结
        public MessageRes MealSummaryByAI(int userID, DateTime date);
    }
}
