using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IFoodPlanBLL
    {
        // 创建一条饮食计划
        public FoodPlanRes Create(FoodPlanInfo foodPlanInfo);

        // 获取用户的全部饮食计划
        public GetAllFoodPlanDetailsRes GetAllDetails(int userID);

        // 删
        public MessageRes DeleteFoodPlan(int foodPlanID);

        // 更新
        public MessageRes UpdateFoodPlan(UpdateFoodPlanInfo updateFoodPlanInfo);

        // 更新完成状态
        public MessageRes UpdateFoodPlanState(int foodPlanID,int state);

        // 预设食物表
        // 增
        public MessageRes InsertFoodInfo(string foodName, int calorie);

        // 删
        public MessageRes DeleteFoodInfo(string foodName);

        // 改
        public MessageRes UpdateFoodInfo(string foodName, int calorie);

        // 查
        public FoodsRes GetALLFoodsInfo();

        // 食谱
        // 增
        public RecipesRes InsertRecipe(RecipesInfo recipesInfo);

        // 删
        public MessageRes DeleteRecipe(int recipeID);

        // 改
        public MessageRes UpdateRecipe(UpdateRecipesInfo updateRecipesInfo);

        // 查
        public Recipes GetAllRecipes();


    }
}
