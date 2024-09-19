using Fitness.BLL;
using Fitness.BLL.Core;
using Fitness.BLL.Interfaces;
using Fitness.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MealPlansController : ControllerBase
    {
        private readonly IFoodPlanBLL _foodPlanBLL;
        private readonly JWTHelper _jwtHelper = new();

        public MealPlansController(IFoodPlanBLL foodPlanBLL)
        {
            _foodPlanBLL = foodPlanBLL;
        }

        // 创建一条饮食计划
        [HttpPost]
        public ActionResult<FoodPlanRes> Create(string token,CreateFoodPlan createFoodPlan)
        {

            Console.WriteLine($"创建饮食计划");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            FoodPlanInfo foodPlanInfo = new()
            {
                userID = userID,
                date = createFoodPlan.date,
                mealType = createFoodPlan.mealType,
                state = createFoodPlan.state,
                foods = createFoodPlan.foods,
            };

            return _foodPlanBLL.Create(foodPlanInfo);
        }


        // 根据userID获取所有饮食计划的详细信息
        [HttpGet]
        public ActionResult<GetAllFoodPlanDetailsRes> GetAllDetails(string token)
        {
            Console.WriteLine($"getALLDetail");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            return _foodPlanBLL.GetAllDetails(userID);
        }

        // 根据foodPlanID删除单条饮食计划
        [HttpDelete]
        public ActionResult<MessageRes> Delete(int foodPlanID)
        {
            Console.WriteLine($"Delete，{foodPlanID}");
            return _foodPlanBLL.DeleteFoodPlan(foodPlanID);
        }

        // 更新饮食计划内容
        [HttpPut]
        public ActionResult<MessageRes> Update(UpdateFoodPlanInfo updateFoodPlanInfo)
        {
            Console.WriteLine($"upDate{updateFoodPlanInfo.foodPlanID}");
            return _foodPlanBLL.UpdateFoodPlan(updateFoodPlanInfo);
        }

        // 更新饮食计划的完成状态
        [HttpPut]
        public ActionResult<MessageRes> UpdateState(UpdateState data)
        {
            Console.WriteLine($"upDateState{data.foodPlanID}");
            return _foodPlanBLL.UpdateFoodPlanState(data.foodPlanID,Convert.ToInt32(data.state));
        }

        // 预设食物表
        // 插入一种食物
        [HttpPost]
        public ActionResult<MessageRes> InsertFoodInfo(Foods food)
        {
            Console.WriteLine($"插入一种食物{food.foodName},{food.calorie}");
            return _foodPlanBLL.InsertFoodInfo(food.foodName,food.calorie);
        }

        // 删除一条食物
        [HttpDelete]
        public ActionResult<MessageRes> DeleteFoodInfo(string foodName)
        {
            Console.WriteLine($"删除一种食物{foodName}");
            return _foodPlanBLL.DeleteFoodInfo(foodName);
        }

        // 更新一条食物
        [HttpPut]
        public ActionResult<MessageRes> UpdateFoodInfo(Foods food)
        {
            Console.WriteLine($"更新一种食物，{food.foodName}");
            return _foodPlanBLL.UpdateFoodInfo(food.foodName, food.calorie);
        }

        // 获取食物数据库中的所有信息
        [HttpGet]
        public ActionResult<FoodsRes> GetFoodsInfo()
        {
            Console.WriteLine($"getFoodsInfo");
            return _foodPlanBLL.GetALLFoodsInfo();
        }

        // 食谱
        // 插入一条食谱
        [HttpPost]
        public ActionResult<RecipesRes> InsertRecipe(RecipesInfo recipesInfo)
        {
            Console.WriteLine($"插入一条食谱");
            return _foodPlanBLL.InsertRecipe(recipesInfo);
        }

        // 删除一条食谱
        [HttpDelete]
        public ActionResult<MessageRes> DeleteRecipe(int recipeID)
        {
            Console.WriteLine($"删除一条食谱：{recipeID}");
            return _foodPlanBLL.DeleteRecipe(recipeID);
        }

        // 更新一条食谱
        [HttpPut]
        public ActionResult<MessageRes> UpdateRecipe(UpdateRecipesInfo updateRecipesInfo)
        {
            Console.WriteLine($"更新一条食谱");
            return _foodPlanBLL.UpdateRecipe(updateRecipesInfo);
        }

        // 查询所有食谱
        [HttpGet]
        public ActionResult<Recipes> GetALLRecipes()
        {
            Console.WriteLine($"getALLRecipes");
            return _foodPlanBLL.GetAllRecipes();
        }



    }
}
