using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class FoodPlanDAL
    {


        // 成功插入一条饮食计划时，返回这条饮食计划的foodPlanID
        public int InsertFoodPlan(FoodPlan foodPlan, OracleTransaction transaction=null)
        {
            int foodPlanID = -1;

            try
            {
                string sql = "INSERT INTO \"FoodPlan\" (\"userID\", \"createTime\", \"date\", \"mealType\", \"state\", \"numOfTypes\")" +
                             " VALUES (:\"userID\", :\"createTime\", :\"date\", :\"mealType\", :\"state\", :\"numOfTypes\")" +
                             " RETURNING \"foodPlanID\" INTO :\"foodPlanID\"";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = foodPlan.userID },
                    new OracleParameter("\"createTime\"", OracleDbType.TimeStamp) { Value = DateTime.Now },
                    new OracleParameter("\"date\"", OracleDbType.Date) { Value = foodPlan.date },
                    new OracleParameter("\"mealType\"", OracleDbType.Int32) { Value = foodPlan.mealType },
                    new OracleParameter("\"state\"", OracleDbType.Int32) { Value = foodPlan.state },
                    new OracleParameter("\"numOfTypes\"", OracleDbType.Int32) { Value = foodPlan.numsOfTypes },
                    new OracleParameter("\"foodPlanID\"", OracleDbType.Int32) { Direction = ParameterDirection.Output }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                foodPlanID = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)oracleParameters[6].Value).Value);

                return foodPlanID;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food plan: {ex.Message}");
                throw;
            }
        }



        public bool InsertFoods(Foods food)
        {
            try
            {
                string sql = "INSERT INTO Foods(foodName, calorie) VALUES(:foodName, :calorie)";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
            new OracleParameter("foodName", OracleDbType.NVarchar2) { Value = food.foodName },
            new OracleParameter("calorie", OracleDbType.Int32) { Value = food.calorie }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food: {ex.Message}");
                throw;
            }
        }


        public bool InsertFoodPlanFood(FoodPlanFood foodPlanFood,OracleTransaction transaction = null)
        {
            try
            {
                string sql = "INSERT INTO \"FoodPlanFood\"(\"foodPlanID\", \"foodName\", \"foodAmount\") " +
                             "VALUES(:\"foodPlanID\", :\"foodName\", :\"foodAmount\")";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"foodPlanID\"", OracleDbType.Int32) { Value = foodPlanFood.foodPlanID },
                    new OracleParameter("\"foodName\"", OracleDbType.NVarchar2) { Value = foodPlanFood.foodName },
                    new OracleParameter("\"foodAmount\"", OracleDbType.Int32) { Value = foodPlanFood.foodAmount }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food plan food: {ex.Message}");
                return false;
            }
        }


        public DataTable GetFoodPlanByUsrID(int userID)
        {
            try
            {
                string sql = "SELECT * FROM \"FoodPlan\" WHERE \"userID\"=:\"userID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = userID });

                return dt;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);     
                return null;
            }
        }

        public DataTable GetFoodPlanFoodsByID(int foodPlanID)
        {
            try
            {
                string sql = "SELECT * FROM \"FoodPlanFood\" WHERE \"foodPlanID\"=:\"foodPlanID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"foodPlanID\"", OracleDbType.Int32) { Value = foodPlanID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public DataTable GetFoodPlanByPlanID(int foodPlanID)
        {
            try
            {
                string sql = "SELECT * FROM \"FoodPlan\" WHERE \"foodPlanID\"=:\"foodPlanID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"foodPlanID\"", OracleDbType.Int32) { Value = foodPlanID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable GetFoodPlanAchieGain(int foodPlanID)
        {
            try
            {
                string sql = "SELECT \"achievementGain\",\"userID\" FROM \"FoodPlan\" WHERE \"foodPlanID\"=:\"foodPlanID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"foodPlanID\"", OracleDbType.Int32) { Value = foodPlanID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public bool UpdateAchievementGain(int foodPlanID,int achievementGain)
        {

            try
            {
                string sql = "UPDATE \"FoodPlan\" SET \"achievementGain\"=:achievementGain " +
                                "WHERE \"foodPlanID\"=:foodPlanID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("achievementGain", OracleDbType.Int32) { Value = achievementGain },
                    new OracleParameter("foodPlanID", OracleDbType.Int32) { Value = foodPlanID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateFoodPlanByPlanID(int foodPlanID,int mealType,int numOfTypes,OracleTransaction transaction=null)
        {

            try
            {
                string sql = "UPDATE \"FoodPlan\" SET \"mealType\"=:mealType, \"numOfTypes\"=:numOfTypes " +
                                "WHERE \"foodPlanID\"=:foodPlanID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    
                    new OracleParameter("mealType", OracleDbType.Int32) { Value = mealType },
                    new OracleParameter("numOfTypes", OracleDbType.Int32) { Value = numOfTypes },
                    new OracleParameter("foodPlanID", OracleDbType.Int32) { Value = foodPlanID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateFoodPlanState(int foodPlanID, int state)
        {

            try
            {
                string sql = "UPDATE \"FoodPlan\" SET \"state\"=:state " +
                                "WHERE \"foodPlanID\"=:foodPlanID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("state", OracleDbType.Int32) { Value = state },
                    new OracleParameter("foodPlanID", OracleDbType.Int32) { Value = foodPlanID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public bool DeleteFoodPlanByPlanID(int foodPlanID)
        {
            try
            {
                string sql = "DELETE FROM \"FoodPlan\" WHERE \"foodPlanID\" =:\"foodPlanID\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("foodPlanID", OracleDbType.Int32) { Value = foodPlanID }
                };
                OracleHelper.ExecuteNonQuery(sql,null, oracleParameters);
                
                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeletFoodPlanFoodByPlanID(int foodPlanID,OracleTransaction transaction=null)
        {
            try
            {
                string sql = "DELETE FROM \"FoodPlanFood\" WHERE \"foodPlanID\" =:\"foodPlanID\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("foodPlanID", OracleDbType.Int32) { Value = foodPlanID }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }

        }

        // 预设食物表
        // 增
        public bool InsertFoods(string foodName,int calorie)
        {
            

            try
            {
                string sql = "INSERT INTO \"Foods\" (\"foodName\", \"calorie\")" +
                             " VALUES (:\"foodName\", :\"calorie\")";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"foodName\"", OracleDbType.NVarchar2) { Value = foodName },
                    new OracleParameter("\"caloire\"", OracleDbType.Int32) { Value = calorie },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food plan: {ex.Message}");
                return false;
            }
        }


        // 删
        public bool DeleteFoodByName(string foodName)
        {
            try
            {
                string sql = "DELETE FROM \"Foods\" WHERE \"foodName\" =:\"foodName\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("foodName", OracleDbType.NVarchar2) { Value = foodName }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }

        }
        // 改
        public bool UpdateFoods(string foodName, int Calorie)
        {

            try
            {
                string sql = "UPDATE \"Foods\" SET \"calorie\"=:Calorie " +
                                "WHERE \"foodName\"=:foodName";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("Calorie", OracleDbType.Int32) { Value = Calorie },
                    new OracleParameter("foodName", OracleDbType.NVarchar2) { Value = foodName },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }
        // 查
        public DataTable GetALLFoodInfoData()
        {
            try
            {
                string sql = "SELECT * FROM \"Foods\" ";
                OracleParameter[] oracleParameters = new OracleParameter[]{};
                DataTable dt = OracleHelper.ExecuteTable(sql, oracleParameters);
                

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }



        // 食谱表
        // 增
        public int InsertRecipes(RecipesInfo recipesInfo,ref DateTime releaseTime)
        {
            int recipeID = -1;
            releaseTime = DateTime.Now;
            try
            {
                string sql = "INSERT INTO \"Recipes\" (\"title\", \"imgUrl\", \"content\", \"releaseTime\")" +
                             " VALUES (:\"title\", :\"imgUrl\", :\"content\",:\"releaseTime\")" +
                             " RETURNING \"recipeID\" INTO :\"recipeID\"";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"title\"", OracleDbType.NVarchar2) { Value = recipesInfo.title },
                    new OracleParameter("\"imgUrl\"", OracleDbType.Clob) { Value = recipesInfo.imgUrl },
                    new OracleParameter("\"content\"", OracleDbType.Clob) { Value = recipesInfo.content },
                    new OracleParameter("\"releaseTime\"", OracleDbType.TimeStamp){ Value = releaseTime },
                    new OracleParameter("\"recipeID\"", OracleDbType.Int32) { Direction = ParameterDirection.Output }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                recipeID = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)oracleParameters[4].Value).Value);

                return recipeID;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food plan: {ex.Message}");
                throw;
            }
        }

        // 删
        public bool DeleteRecipeByID(int recipeID)
        {
            try
            {
                string sql = "DELETE FROM \"Recipes\" WHERE \"recipeID\" =:\"recipeID\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("recipeID", OracleDbType.Int32) { Value = recipeID }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }

        }

        // 改
        public bool UpdateRecipes(UpdateRecipesInfo updateRecipesInfo)
        {

            try
            {
                string sql = "UPDATE \"Recipes\" SET \"title\"=:tile \"imgUrl\"=:imgUrl \"content\"=:content \"releaseTime\"=:releaseTime " +
                                "WHERE \"recipeID\"=:recipeID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("title", OracleDbType.Clob) { Value = updateRecipesInfo.title },
                    new OracleParameter("imgUrl", OracleDbType.Clob) { Value = updateRecipesInfo.imgUrl },
                    new OracleParameter("content", OracleDbType.Clob) { Value = updateRecipesInfo.content },
                    new OracleParameter("releaseTime", OracleDbType.TimeStamp){ Value = DateTime.Now },
                    new OracleParameter("recipeID", OracleDbType.Int32) { Value = updateRecipesInfo.recipeID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 查
        public DataTable GetAllRecipes()
        {
            try
            {
                string sql = "SELECT * FROM \"Recipes\" ";
                OracleParameter[] oracleParameters = new OracleParameter[] { };
                DataTable dt = OracleHelper.ExecuteTable(sql, oracleParameters);

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


    }
}
