using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class RecordInfo
    {
        public DateTime mealTime { get; set; }

        public string? mealPhoto { get; set; }

        public List<Food> foods { get; set; }
    }

    public class CreateMealRecord
    {
        public DateTime mealTime { get; set; }

        public string? mealPhoto { get; set; }

        public List<Food> foods { get; set; }
    }

    public class MealRecordInfo
    {
        public int userID { get; set; }

        public DateTime mealTime { get; set; }

        public string? mealPhoto { get; set; }

        public List<Food> foods { get; set; }
    }

    // 定义创建饮食记录功能-后端响应格式
    public class MealRecordRes
    {
        public string message { get; set; }

        public int recordID { get; set; }
        
        public int totalCalorie { get; set; }
    }

    public class AISuggestionRes
    {
        public string diningAdvice { get; set; }

  
    }
    public class FoodRecord
    {
        public string foodName { get; set; }

        public int quantity { get; set; }

        public int calorie { get; set; }
    }

    public class MealRecordFood
    {
        public int recordID { get; set; }

        public string foodName { get; set; }

        public int foodAmount { get; set; }
    }


    public class SingleMealRecordNoDetail
    {
        public int recordID { get; set; }

        public DateTime mealTime { get; set; }

        public string mealPhoto { get; set; }

        public DateTime createdTime { get; set; }

    }
    
    public class GetAllMealRecordsNoDetailsRes
    {
        public List<SingleMealRecordNoDetail> records { get; set; } = new();

    }

    public class GetOneMealRecordDetailsRes
    {
        public int recordID { get; set; }

        public List<FoodRecord> foods { get; set; } = new();

        public DateTime mealTime { get; set; }  

        public string mealPhoto { get; set; }

        public DateTime createdTime { get; set; }

        public int totalCalorie { get; set; }

        public string diningAdvice { get; set; }


    }

    public class GetAllMealRecordsDetailsRes
    {
        public List<GetOneMealRecordDetailsRes> records { get; set; } = new();

    }

    public class UpdateMealRecordInfo
    {
        public List<Food> foods { get; set; } = new();

        public string mealPhoto { get; set; }

        public DateTime mealTime { get; set; }

        public int recordID { get; set; }
    }



}
