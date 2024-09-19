using Fitness.BLL.Interfaces;
using Fitness.DAL;
using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL
{
    public class FoodPlanBLL : IFoodPlanBLL
    {
        FoodPlanDAL foodPlanDAL = new();

        // 创建一条饮食记录
        public FoodPlanRes Create(FoodPlanInfo foodPlanInfo)
        {
            FoodPlanRes foodPlanRes = new()
            {
                message = "饮食计划插入失败",
                foodPlanID = -1
            };

            FoodPlan foodPlan = new()
            {
                foodPlanID = 0,
                userID = foodPlanInfo.userID,
                createTime = new DateTime(1919, 10, 12),
                date = foodPlanInfo.date,
                mealType = foodPlanInfo.mealType,
                state = foodPlanInfo.state,
                numsOfTypes = foodPlanInfo.foods.Count

            };

            int foodPlanID = -1;

            using (var connection = OracleHelper.GetConnection())
            {
                OracleTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    
                    // 插入一级缩略表
                    foodPlanID = foodPlanDAL.InsertFoodPlan(foodPlan,transaction);
                    

                    if (foodPlanID==-1)
                    {
                        foodPlanRes.message = "饮食计划插入失败:一级缩略表插入错误！";
                        transaction.Rollback();
                        return foodPlanRes;
                    }

                    // 插入详细的食物列表
                    for (int i = 0; i < foodPlanInfo.foods.Count; i++)
                    {

                        FoodPlanFood foodPlanFood = new()
                        {
                            foodPlanID = foodPlanID,
                            foodName = foodPlanInfo.foods[i].foodName,
                            foodAmount = foodPlanInfo.foods[i].quantity
                        };
                        if(!foodPlanDAL.InsertFoodPlanFood(foodPlanFood,transaction))
                        {
                            foodPlanRes.message = "饮食计划插入失败:详细食物列表插入错误！";
                            transaction.Rollback();
                            return foodPlanRes;
                        }
                    }

                    transaction.Commit();
                    foodPlanRes.message = "饮食计划插入成功";
                    foodPlanRes.foodPlanID = foodPlanID;

                    return foodPlanRes;
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine($"插入饮食计划时出错:{ex.Message}");
                    foodPlanRes.message = $"插入饮食计划时出错:{ex.Message}";
                    return foodPlanRes;
                }
            }
        }

        
        public GetAllFoodPlanDetailsRes GetAllDetails(int userID)
        {
            DataTable dt = foodPlanDAL.GetFoodPlanByUsrID(userID);
            GetAllFoodPlanDetailsRes res = new();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GetOneFoodPlanDetailRes single = new();
                DataRow dr = dt.Rows[i];
                single.foodPlanID = Convert.ToInt32(dr["foodPlanID"]);
                single.date = Convert.ToDateTime(dr["date"]);
                single.mealType = Convert.ToInt32(dr["mealType"]);
                single.state = Convert.ToInt32(dr["state"]);
                single.numOfTypes = Convert.ToInt32(dr["numOfTypes"]);

                DataTable dtDetailFoods = foodPlanDAL.GetFoodPlanFoodsByID(single.foodPlanID);
                for(int j= 0;j<dtDetailFoods.Rows.Count;j++)
                {
                    Food food = new();
                    DataRow drDetailFoods = dtDetailFoods.Rows[j];
                    food.foodName = drDetailFoods["foodName"].ToString();
                    food.quantity = Convert.ToInt32(drDetailFoods["foodAmount"]);
                    single.foods.Add(food);
                }
                res.plans.Add(single);

            }
            return res;
        }


        public MessageRes DeleteFoodPlan(int foodPlanID)
        {
            MessageRes deleteFoodPlanRes = new();
            
            bool res = foodPlanDAL.DeleteFoodPlanByPlanID(foodPlanID);

            if (res)
                deleteFoodPlanRes.message = "饮食计划删除成功";
            else
                deleteFoodPlanRes.message = "饮食计划删除失败";

            return deleteFoodPlanRes;

        }


        public MessageRes UpdateFoodPlan(UpdateFoodPlanInfo updateFoodPlanInfo)
        {
            MessageRes res = new MessageRes();
            
            int foodPlanID = updateFoodPlanInfo.foodPlanID;
            int mealType = updateFoodPlanInfo.mealType;
            int numOfTypes = updateFoodPlanInfo.foods.Count;
            Console.WriteLine($"foodPlanID{foodPlanID}");
            using (var connection = OracleHelper.GetConnection())
            {
                OracleTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    if (!foodPlanDAL.UpdateFoodPlanByPlanID(foodPlanID, mealType, numOfTypes))
                    {
                        res.message = "饮食计划更新失败:缩略表信息更新错误！";
                        return res;
                    }
                    // Console.WriteLine($"foodPlanID2{foodPlanID}");
                    if (!foodPlanDAL.DeletFoodPlanFoodByPlanID(foodPlanID,transaction))
                    {
                        res.message = "饮食计划更新失败:旧食物详情表删除失败！";
                        transaction.Rollback();
                        return res;
                    }
                    // Console.WriteLine($"foodPlanID3{foodPlanID}");

                    for (int i = 0; i < updateFoodPlanInfo.foods.Count; i++)
                    {

                        FoodPlanFood foodPlanFood = new()
                        {
                            foodPlanID = foodPlanID,
                            foodName = updateFoodPlanInfo.foods[i].foodName,
                            foodAmount = updateFoodPlanInfo.foods[i].quantity
                        };
                        
                        if (!foodPlanDAL.InsertFoodPlanFood(foodPlanFood))
                        {
                            res.message = "饮食计划更新失败:新食物详情信息插入失败！";
                            transaction.Rollback();
                            return res;
                        }
                        Console.WriteLine($"新食物详情表插入");
                    }

                    transaction.Commit();
                    res.message = "饮食计划更新成功！";
                    return res;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"饮食计划更新失败:{ex.Message}");
                    transaction?.Rollback();
                    
                    res.message = $"饮食计划更新失败:{ex.Message}";
                    return res;
                }
            }
        }

        public MessageRes UpdateFoodPlanState(int foodPlanID, int state)
        {
            MessageRes res = new MessageRes();


            Console.WriteLine($"更新状态{foodPlanID}");
            if (!foodPlanDAL.UpdateFoodPlanState(foodPlanID, state))
            {
                res.message = "饮食计划状态更新失败！(12)";
                return res;
            }


            res.message = "饮食计划状态更新成功！";
            
            // 确保每条饮食计划只能更新一次成就进度
            DataTable dt = foodPlanDAL.GetFoodPlanAchieGain(foodPlanID);
            DataRow dr = dt.Rows[0];
            int haveGainAchievement = Convert.ToInt32(dr["achievementGain"]);
            int userID = Convert.ToInt32(dr["userID"]);


            // 完成饮食计划更新成就进度
            if (state==1 && haveGainAchievement==0)
            {
                UserAchievementBLL userAchievementBLL = new();
                foodPlanDAL.UpdateAchievementGain(foodPlanID, 1);
                userAchievementBLL.UpdateFoodPlanAchievement(userID);
            }

            return res;
        }

        // 预设食物表
        // 增
        public MessageRes InsertFoodInfo(string foodName,int calorie)
        {
            MessageRes res = new()
            {
                message = "食物插入失败！",
            };

            if (foodPlanDAL.InsertFoods(foodName, calorie))
                res.message = "食物插入成功！";

            return res;

        }

        // 删
        public MessageRes DeleteFoodInfo(string foodName)
        {
            MessageRes res = new()
            {
                message = "食物删除失败！"
            };

           
            if (foodPlanDAL.DeleteFoodByName(foodName))
                res.message = "食物删除成功！";


            return res;

        }
        // 改
        public MessageRes UpdateFoodInfo(string foodName, int calorie)
        {
            MessageRes res = new()
            {
                message = "食物表更新失败！"
            };


            if (foodPlanDAL.UpdateFoods(foodName, calorie))
            {
                res.message = "食物表更新成功！";
                
            }

            return res;

        }

        // 查
        public FoodsRes GetALLFoodsInfo()
        {
            DataTable dt = foodPlanDAL.GetALLFoodInfoData();
            FoodsRes foodsRes = new();
            

            for (int i=0;i<dt.Rows.Count;i++)
            {
                DataRow dr = dt.Rows[i];
                FoodInfo foodInfo = new();
                foodInfo.foodName = dr["foodName"].ToString();
                foodInfo.calorie = Convert.ToInt32(dr["calorie"]);
                foodsRes.foodsInfo.Add(foodInfo);
            }
            return foodsRes;
        }

        // 食谱

        // 增
        public RecipesRes InsertRecipe(RecipesInfo recipesInfo)
        {
            RecipesRes recipesRes = new()
            {
                message = "食谱插入失败！",
                recipeID = -1,
                releaseTime = DateTime.Now
            };

            long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond;
            // 使用时间戳创建唯一的 objectName
            string objectName = $"{recipesInfo.title}_{timestamp}.png";
            int base64Index = recipesInfo.imgUrl.IndexOf("base64,") + 7;
            recipesInfo.imgUrl = recipesInfo.imgUrl.Substring(base64Index);
            OSSHelper.UploadBase64ImageToOss(recipesInfo.imgUrl, objectName);
            recipesInfo.imgUrl = OSSHelper.GetPublicObjectUrl(objectName);

            DateTime releaseTime = DateTime.Now; 
            int recipeID = foodPlanDAL.InsertRecipes(recipesInfo, ref releaseTime);

            if (recipeID == -1)
                return recipesRes;
            else
            {
                recipesRes.message = "食谱插入成功！";
                recipesRes.recipeID = recipeID;
                recipesRes.releaseTime = releaseTime;
                return recipesRes;
            }

        }


        // 删
        public MessageRes DeleteRecipe(int recipeID)
        {
            MessageRes deleteRecipeRes = new()
            {
                message = "食谱删除失败！"
            };

            bool res = foodPlanDAL.DeleteRecipeByID(recipeID);

            if (res)
                deleteRecipeRes.message = "食谱删除成功！";


            return deleteRecipeRes;

        }

        // 改
        public MessageRes UpdateRecipe(UpdateRecipesInfo updateRecipesInfo)
        {
            MessageRes res = new MessageRes();


            if (!foodPlanDAL.UpdateRecipes(updateRecipesInfo))
            {
                res.message = "食谱更新失败！";
                return res;
            }


            res.message = "食谱更新成功！";
            return res;
        }

        // 查
        public Recipes GetAllRecipes()
        {
            DataTable dt = foodPlanDAL.GetAllRecipes();
            Recipes recipes = new();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                SingleRecipe singleRecipe = new();
                singleRecipe.title = dr["title"].ToString();
                singleRecipe.imgUrl = dr["imgUrl"].ToString();
                singleRecipe.content = dr["content"].ToString();
                singleRecipe.releaseTime = Convert.ToDateTime(dr["releaseTime"]);
                singleRecipe.recipeID = Convert.ToInt32(dr["recipeID"]);

                recipes.recipes.Add(singleRecipe);
            }
            return recipes;
        }

    }
}
