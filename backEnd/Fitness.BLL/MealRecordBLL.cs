using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Net.Http;
//using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Fitness.BLL.Interfaces;
using Fitness.DAL;
using System.Data;
using Fitness.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Fitness.BLL.Core;
using System.Runtime.CompilerServices;

namespace Fitness.BLL
{
    public class MealRecordBLL:IMealRecordBLL
    {
        MealRecordDAL mealRecordDAL = new();
        VigorTokenBLL vigorTokenBLL = new();

        string prePrompt = "- Role: 健身系统内的饮食健康专家\r\n" +
            "- Background: 用户希望通过上传饮食图像和提供饮食信息来获得专业的饮食建议，以帮助他们达到健身目标并改善健康状况。\r\n" +
            "- Profile: 你是一位经验丰富的营养师，具备深厚的营养学知识和丰富的实践经验，能够通过分析用户的饮食内容提供个性化的饮食建议。\r\n" +
            "- Skills: 营养学知识、饮食分析、健康建议、沟通技巧。\r\n- Goals: 为用户提供科学的饮食建议，帮助他们实现健身目标并维持健康的生活方式。\r\n" +
            "- Constrains: 建议应基于用户上传的饮食图像 \r\n- OutputFormat: 不要有任何多余的问候语，直接给出建议！\r\n" +
            "- Workflow:\r\n  " +
            "1. 分析用户上传的饮食图像和信息。\r\n  " +
            "2. 根据营养学原则评估饮食的均衡性和健康性。\r\n  " +
            "3. 提供针对性的饮食建议和改进方案。\r\n  " +
            "4. 给出未来餐饮的建议，包括食材选择、饮食结构和饮食习惯。"+
            "用户的餐饮信息如下：";


        // 创建一条饮食记录
        public MealRecordRes Create(MealRecordInfo mealRecordInfo)
        {
            int recordID = -1;
            MealRecordRes mealRecordRes = new()
            {
                message = "饮食记录插入失败！",
                recordID = -1
            };

            // 使用时间戳创建唯一的 objectName
            long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond;
            string objectName = $"{mealRecordInfo.userID}_{timestamp}.png";
            int base64Index = mealRecordInfo.mealPhoto.IndexOf("base64,") + 7;
            mealRecordInfo.mealPhoto = mealRecordInfo.mealPhoto.Substring(base64Index);
            OSSHelper.UploadBase64ImageToOss(mealRecordInfo.mealPhoto, objectName);
            mealRecordInfo.mealPhoto = OSSHelper.GetPublicObjectUrl(objectName);

            using (var connection = OracleHelper.GetConnection())
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    recordID = mealRecordDAL.InsertMealRecord(mealRecordInfo,transaction);
                    if(recordID==-1)
                    {
                        transaction.Rollback();
                        return mealRecordRes;
                    }

                    int totalCalorie = 0;

                    for (int i = 0; i < mealRecordInfo.foods.Count; i++)
                    {
                        prePrompt += mealRecordInfo.foods[i].foodName + ":" + mealRecordInfo.foods[i].quantity.ToString() + "克;";

                        if(mealRecordDAL.CheckFoodExists(mealRecordInfo.foods[i].foodName))
                        {
                            DataTable dtCalorie = mealRecordDAL.GetOneFoodCalorie(mealRecordInfo.foods[i].foodName);
                            DataRow drCalorie = dtCalorie.Rows[0];
                            totalCalorie += Convert.ToInt32(drCalorie["calorie"]) * mealRecordInfo.foods[i].quantity;
                        }
                        
                        
                        MealRecordFood mealRecordFood = new()
                        {
                            recordID = recordID,
                            foodName = mealRecordInfo.foods[i].foodName,
                            foodAmount = mealRecordInfo.foods[i].quantity
                        };

                        if(!mealRecordDAL.InsertMealRecordsFood(mealRecordFood))
                        {
                            transaction.Rollback();
                            return mealRecordRes;
                        }

                    }

                    mealRecordRes.message = "饮食记录插入成功！";
                    mealRecordRes.recordID = recordID;
                    mealRecordRes.totalCalorie = totalCalorie;
                    transaction.Commit();

                    // 异步任务在后台执行
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            // 大模型生成
                            string result = await Qwen_VL.CallQWenVL(prePrompt, mealRecordInfo.mealPhoto);

                            // 更新建议
                            mealRecordDAL.UpdateAdvicing(recordID, result);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"后台处理失败: {ex.Message}");
                        }
                    });

                    return mealRecordRes;

                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine($"创建饮食记录失败:{ex.Message}");
                    mealRecordRes.message = $"创建饮食记录失败:{ex.Message}";
                    return mealRecordRes;

                }
            }
        }

        public AISuggestionRes GetAISuggestion(int recordID)
        {
            DataTable dt = mealRecordDAL.GetAISuggestionByRecordID(recordID);
            AISuggestionRes res = new();

            DataRow dr = dt.Rows[0];
            res.diningAdvice = dr["diningAdvice"].ToString();
            return res;
        }


        // 根据日期获取所有的饮食记录
        public GetAllMealRecordsDetailsRes GetAllDetails(int userID,DateTime date)
        {
            DataTable dt = mealRecordDAL.GetMealRecordByIDAndDate(userID,date);
            GetAllMealRecordsDetailsRes res = new();

            

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GetOneMealRecordDetailsRes single = new();
                DataRow dr = dt.Rows[i];
                single.recordID = Convert.ToInt32(dr["recordID"]);
                single.mealTime = Convert.ToDateTime(dr["mealTime"]);
                single.mealPhoto = dr["mealPhoto"].ToString();
                single.createdTime = Convert.ToDateTime(dr["createTime"]);
                single.diningAdvice = dr["diningAdvice"].ToString();

                int totalcalorie = 0;
                
                

                DataTable dtDetailFoods = mealRecordDAL.GetMealRecordsFoodsByID(single.recordID);


                for (int j = 0; j < dtDetailFoods.Rows.Count; j++)
                {
                    FoodRecord food = new();
                    DataRow drDetailFoods = dtDetailFoods.Rows[j];
                    food.foodName = drDetailFoods["foodName"].ToString();
                    food.quantity = Convert.ToInt32(drDetailFoods["foodAmount"]);
                    if(mealRecordDAL.CheckFoodExists(food.foodName))
                    {
                        DataTable dtCalorie = mealRecordDAL.GetOneFoodCalorie(food.foodName);
                        DataRow drCalorie = dtCalorie.Rows[0];
                        food.calorie = Convert.ToInt32(drCalorie["calorie"]) * food.quantity; // 这个食物的所有calorie
                        totalcalorie += food.calorie;
                    }
                    single.foods.Add(food);
                }

                single.totalCalorie = totalcalorie/100;
                
                res.records.Add(single);

            }
            return res;
        }

        
        // 删除饮食记录
        public MessageRes DeleteMealRecord(int recordID)
        {
            MessageRes deleteRes = new();

            bool res = mealRecordDAL.DeleteMealRecordByRecordID(recordID);

            if (res)
                deleteRes.message = "饮食计划删除成功";
            else
                deleteRes.message = "饮食计划删除失败";

            return deleteRes;

        }

        // 更新饮食记录
        public MessageRes UpdateMealRecords(UpdateMealRecordInfo updateMealRecordInfo)
        {
            MessageRes res = new MessageRes();

            int recordID = updateMealRecordInfo.recordID;
            DateTime mealTime = updateMealRecordInfo.mealTime;
            string mealPhoto = updateMealRecordInfo.mealPhoto;
            Console.WriteLine(mealPhoto);

            long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond;

            // 使用时间戳创建唯一的 objectName
            string objectName = $"{recordID}_{timestamp}.png";
            int base64Index = mealPhoto.IndexOf("base64,") + 7;
            Console.WriteLine(base64Index);
            if (base64Index != 6)
            {
                mealPhoto = mealPhoto.Substring(base64Index);
                OSSHelper.UploadBase64ImageToOss(mealPhoto, objectName);
                mealPhoto = OSSHelper.GetPublicObjectUrl(objectName);
            }
            

            using (var connection = OracleHelper.GetConnection())
            {
                OracleTransaction transaction = null;
                
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    if (!mealRecordDAL.UpdateMealRecordByRecordID(recordID, mealTime, mealPhoto,transaction))
                    {
                        transaction.Rollback();
                        res.message = "饮食记录更新失败！--删除缩略信息时出错！";
                        return res;
                    }

                    if (!mealRecordDAL.DeletMealRecordFoodByID(recordID,transaction))
                    {
                        transaction.Rollback();
                        res.message = "饮食记录更新失败！--删除旧详细食物信息时出错！";
                        return res;
                    }

                    for (int i = 0; i < updateMealRecordInfo.foods.Count; i++)
                    {

                        MealRecordFood mealRecordFood = new()
                        {
                            recordID = recordID,
                            foodName = updateMealRecordInfo.foods[i].foodName,
                            foodAmount = updateMealRecordInfo.foods[i].quantity
                        };
                        if(!mealRecordDAL.InsertMealRecordsFood(mealRecordFood))
                        {
                            transaction.Rollback();
                            res.message = "饮食记录更新失败！--插入新详细食物信息时出错！";
                            return res;
                        }
                    }

                    transaction.Commit();
                    res.message = "饮食记录更新成功！";

                    

                    // 异步任务在后台执行
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            // 大模型生成
                            string result = await Qwen_VL.CallQWenVL(prePrompt, mealPhoto);

                            // 更新建议
                            mealRecordDAL.UpdateAdvicing(recordID, result);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"后台处理失败: {ex.Message}");
                        }
                    });

                    return res;

                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine($"饮食记录更新失败:{ex.Message} ");
                    res.message = $"饮食记录更新失败:{ex.Message} ";
                    return res;

                }
            }
        }

        // 根据日期获取所有饮食记录的缩略信息
        public GetAllMealRecordsNoDetailsRes GetAllNoDetailsByDate(int userID, DateTime date)
        {
            DataTable dt = mealRecordDAL.GetMealRecordByIDAndDate(userID,date);
            GetAllMealRecordsNoDetailsRes res = new();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SingleMealRecordNoDetail single = new();
                DataRow dr = dt.Rows[i];
                single.recordID = Convert.ToInt32(dr["recordID"]);
                single.mealTime = Convert.ToDateTime(dr["mealTime"]);
                single.mealPhoto = dr["mealPhoto"].ToString();
                single.createdTime = Convert.ToDateTime(dr["createTime"]);
                res.records.Add(single);

            }
            return res;

        }

        // 获取AI对饮食记录的当日总结
        public MessageRes MealSummaryByAI(int userID,DateTime date)
        {
            vigorTokenBLL.UpdateBalance(userID, "使用AI总结当日饮食记录功能,耗费30活力币", -30);

            string sys = "- Role: 饮食管理专家\r\n" +
                "- Background: 用户希望对当天的饮食进行详细的总结和评估，包括营养摄入和热量摄入。\r\n" +
                "- Profile: 你是一位专业的饮食管理专家，具有丰富的营养学知识和对食物热量的深刻理解。\r\n" +
                "- Skills: 营养学知识、热量计算、饮食评估、健康建议。\r\n" +
                "- Goals: 对用户上传的饮食记录进行总结，评估营养摄入情况，并给出建议。\r\n" +
                "- Constrains: 总结应简洁明了，易于理解，同时提供科学合理的建议。\r\n" +
                "- OutputFormat: 用中文问答！必须用中文回答！总结报告，分点总结,可以先分每一餐，再全局看整天的情况，多维度分析和建议。\r\n" +
                "- Workflow:\r\n  " +
                "1. 接收并分析用户上传的饮食记录。\r\n  " +
                "2. 计算总热量摄入和各类营养素的摄入量。\r\n  " +
                "3. 从蛋白质、维生素、矿物质等多维度进行总结。\r\n  " +
                "4. 根据总结结果，提供简短的健康饮食建议。";

            MessageRes res = new();
            GetAllMealRecordsDetailsRes records = GetAllDetails(userID, date);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(records, options);

            string Prompt = "我的当天饮食记录结构化数据如下: "+jsonString;
            Console.WriteLine(jsonString);
            res.message = Qwen_VL.CallQWenSingle(sys,Prompt);

            return res;

            
        }

    }
}
