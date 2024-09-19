using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    // 食物表类
    public class Foods
    {
        public string foodName { get; set; }

        public int calorie { get; set; }
    }

    // 饮食记录食物信息
    public class MealRecordsFood
    {
        public int recordID { get; set; }

        public string foodName { get; set; }

        public int foodAmount { get; set; }

    }

    // 饮食计划
    public class FoodPlan
    {
        public int? foodPlanID { get; set; }

        public int userID { get; set; }

        public DateTime? createTime { get; set; }

        public DateTime? date { get; set; }

        public int mealType { get; set; }

        public bool state { get; set; }

        public int? numsOfTypes { get; set; }
    }

    // 饮食计划中单个食物信息
    public class FoodPlanFood
    {
        public int foodPlanID { get; set; }

        public string foodName { get; set; }

        public int foodAmount { get; set; }
    }

    
    public class Food
    {
        public string foodName { get; set; }
        public int quantity { get; set; }
    }

    // 定义创建饮食计划功能-前端发送数据格式
    public class CreateFoodPlan
    {
        public DateTime? date { get; set; }
        public int mealType { get; set; }
        public bool state { get; set; }
        public List<Food> foods { get; set; }
    }
 
    public class FoodPlanInfo
    {
        public int userID { get; set; }
        public DateTime? date { get; set; }
        public int mealType { get; set; }
        public bool state { get; set; }
        public List<Food> foods { get; set; }

    }

    // 定义创建饮食计划功能-后端响应格式
    public class FoodPlanRes
    {
        public string message { get; set; }

        public int foodPlanID { get; set; }
    }


    // 单条计划缩略信息
    public class SinglePlanNoDetail
    {
        public int foodPlanID { get; set; }
        public DateTime date { get; set; }
        public int mealType { get; set; }
        public int state { get; set; }
        public int numOfTypes { get; set; }
    }

    // 定义获取某个用户所有饮食计划缩略信息-后端响应
    public class GetAllFoodPlanNoDetailsRes 
    {

        public List<SinglePlanNoDetail> plans { get; set; } = new ();
    }

    // 单条饮食计划详细信息返回值
    public class GetOneFoodPlanDetailRes
    {
        public int foodPlanID { get; set; }
        public DateTime date { get; set; }
        public int mealType { get; set; }
        public int state { get; set; }
        public int numOfTypes { get; set; }
        public List<Food> foods { get; set; } = new();

    }

    // 定义获取某个用户所有饮食计划详细信息-后端响应
    public class GetAllFoodPlanDetailsRes
    {

        public List<GetOneFoodPlanDetailRes> plans { get; set; } = new();
    }


    // 定义更新FoodPlan的前端发送数据格式
    public class UpdateFoodPlanInfo
    {
        public int foodPlanID { get; set; }
        public int mealType { get; set; }
        public List<Food> foods { get; set; } = new();
    }

    // 统一回复模板
    public class MessageRes
    {
        public string message { get; set; }
    }

    // 预设食物表信息
    public class FoodInfo
    {
        public string foodName { get;set; }
        public int calorie { get; set; }
    }

    // 预设食物表批量返回信息
    public class FoodsRes
    {
        public List<FoodInfo> foodsInfo { get; set; } = new();
    }

    // 食谱
    // 插入请求体
    public class RecipesInfo
    {
        public string title { get; set; }

        public string imgUrl { get; set; }

        public string content { get; set; }
    }
    // 插入响应
    public class RecipesRes
    {
        public string message { get; set; }

        public int recipeID { get; set;}

        public DateTime releaseTime { get; set; }
    }
    public class UpdateRecipesInfo
    {
        public int recipeID { get; set; }

        public string title { get; set; }

        public string imgUrl { get; set; }

        public string content { get; set; }
    }
    //  单条食谱信息
    public class SingleRecipe
    {
        public int recipeID { get; set; }

        public string title { get; set; }

        public string imgUrl { get; set; }

        public string content { get; set; }

        public DateTime releaseTime { get; set; }
    }

    // 批量食谱返回信息
    public class Recipes
    {
        public List<SingleRecipe> recipes { get; set; } = new();
    }

    public class UpdateState
    {
        public int foodPlanID { get;set;}
        public bool state { get; set; }
    }
}
