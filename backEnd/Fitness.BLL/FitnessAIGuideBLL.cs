using Fitness.BLL.Interfaces;
using Fitness.DAL;
using System.Data;
using Fitness.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.Models;
using Fitness.BLL.Core;
using Fitness.DAL.Core;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Fitness.BLL
{
    public class FitnessAIGuideBLL:IFitnessALGuideBLL
    {
        FitnessAIGuideDAL fitnessAIGuideDAL = new();
        VigorTokenBLL vigorTokenBLL = new();

        string prePrompt = "- Role: 健身教练\r\n" +
            "- Background: 用户希望通过获得专业的动作分析与调整建议。\r\n" +
            "- Profile: 你是一位经验丰富的健身教练，拥有专业的人体运动学知识和丰富的实践经验。\r\n" +
            "- Skills: 人体运动学、运动技术分析、运动安全指导、个性化训练建议。\r\n" +
            "- Goals: 提供准确的动作分析，确保用户的动作标准，避免运动损伤，并给出针对性的改进建议。\r\n" +
            "- Constrains: 分析应基于科学依据，建议应易于用户理解和执行;字数不要太多，表述要精简\r\n " +
            "- OutputFormat: 不要有任何多余的问候语，直接给出动作分析结果和具体调整建议！\r\n" +
            "- Workflow:\r\n  " +
            "1. 分析图中的动作是否标准，识别关键的运动部位和执行细节。\r\n  " +
            "2. 根据分析结果，给出动作调整的具体建议。\r\n  " +
            "图中的健身动作是";

        public ScreenshotRes Create(ScreenshotInfo screenshotInfo)
        {

            vigorTokenBLL.UpdateBalance(screenshotInfo.userID, "使用AI健身教练功能，耗费50活力币", -50);
            ScreenshotRes putScreenshotRes = new()
            {
                message = "截图上传失败！",
                screenshotID = -1,
                createTime = DateTime.Now,
            };
            long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond;
            // 使用时间戳创建唯一的 objectName,转化图片为url
            string objectName = $"{screenshotInfo.userID}_{timestamp}.png";
            int base64Index = screenshotInfo.screenshotUrl.IndexOf("base64,") + 7;
            screenshotInfo.screenshotUrl = screenshotInfo.screenshotUrl.Substring(base64Index);
            OSSHelper.UploadBase64ImageToOss(screenshotInfo.screenshotUrl, objectName);
            screenshotInfo.screenshotUrl = OSSHelper.GetPublicObjectUrl(objectName);
            int screenshotID = fitnessAIGuideDAL.InsertFitnessSuggestion(screenshotInfo);
            if (screenshotID != -1)
            {
                putScreenshotRes.message = "截图上传成功！";
                putScreenshotRes.screenshotID = screenshotID;
                putScreenshotRes.screenshotUrl = screenshotInfo.screenshotUrl;

                // 异步任务在后台执行
                _ = Task.Run(async () =>
                {
                    try
                    {
                        prePrompt += screenshotInfo.exerciseName;
                        // 大模型生成
                        string result = await Qwen_VL.CallQWenVL(prePrompt, screenshotInfo.screenshotUrl);

                        // 更新建议
                        fitnessAIGuideDAL.UpdateSuggestion(screenshotID, result);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"后台处理失败: {ex.Message}");
                    }
                });
            }
            return putScreenshotRes;
        }

        // 获取AI的分析
        public MessageRes GetAISuggestion(int screenshotID)
        {
            MessageRes res = new();
            DataTable dt =  fitnessAIGuideDAL.GetSuggestionByID(screenshotID);
            DataRow dr = dt.Rows[0];
            res.message = dr["suggestions"].ToString();

            return res;

        }

        // 获取所有的健身截图记录
        public SuggestionDetailRes GetAllDetails(int userID)
        {
            DataTable dt = fitnessAIGuideDAL.GetFitnessSuggestionByUsrID(userID);
            SuggestionDetailRes res = new();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SingleDetailSuggestionRes single = new();
                DataRow dr = dt.Rows[i];
                single.screenshotID = Convert.ToInt32(dr["screenshotID"]);
                single.createTime = Convert.ToDateTime(dr["createTime"]);
                single.exerciseName = dr["exerciseName"].ToString();
                single.suggestions = dr["suggestions"].ToString();
                single.screenshotUrl = dr["screenshotUrl"].ToString();
                res.suggestions.Add(single);

            }
            return res;
        }

        // 删除单条健身截图记录
        public MessageRes DeleteFitnessSuggestion(int screenshotID)
        {
            MessageRes deleteRes = new();

            bool res = fitnessAIGuideDAL.DeleteFitnessSuggestion(screenshotID);

            if (res)
                deleteRes.message = "健身动作记录删除成功";
            else
                deleteRes.message = "健身动作记录删除失败";

            return deleteRes;

        }

        // 健身器材模块

        // 随机获取7种健身器材简要说明
        public RandomEquiGuidesRes GetRandomEquiGuides()
        {
            DataTable dt = fitnessAIGuideDAL.GetALLEquipments();
            RandomEquiGuidesRes res = new();

            // 创建一个随机数生成器
            Random rand = new Random();

            // 将DataTable中的行转换为列表
            List<DataRow> rows = dt.AsEnumerable().ToList();

            // 随机排序
            rows = rows.OrderBy(row => rand.Next()).ToList();

            // 选择前7个
            for (int i = 0; i < Math.Min(7, rows.Count); i++)
            {
                SingleEquiGuideNoDetail single = new();
                DataRow dr = rows[i];
                single.equipmentName = dr["equipmentName"].ToString();
                single.imgUrl = dr["imgUrl"].ToString();
                single.briefIntr = dr["briefIntr"].ToString();
                res.guides.Add(single);
            }

            return res;
        }

        // 管理员获取全部
        public AllEquiGuidesRes GetALLEquiGuides()
        {
            DataTable dt = fitnessAIGuideDAL.GetALLEquipments();
            AllEquiGuidesRes res = new();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FitnessEquipmentWithTime single = new();
                DataRow dr = dt.Rows[i];
                single.equipmentName = dr["equipmentName"].ToString();
                single.imgUrl = dr["imgUrl"].ToString();
                single.briefIntr = dr["briefIntr"].ToString();
                single.lastUpdateTime = Convert.ToDateTime(dr["lastUpdateTime"]);
                single.operationGuide = dr["operationGuide"].ToString();
                res.guides.Add(single);
            }

            return res;
        }



        // 根据器材名称获取某种器材的详细说明
        public SingleDetailEqui GetOneEquiGuide(string equipmentName)
        {
            DataTable dt = fitnessAIGuideDAL.GetEquipmentByName(equipmentName);
            SingleDetailEqui res = new();
            DataRow dr = dt.Rows[0];

            res.equipmentName = dr["equipmentName"].ToString();
            res.operationGuide = dr["operationGuide"].ToString();
            res.imgUrl = dr["imgUrl"].ToString();

            return res;
        }

        // 插入健身器材
        public FitnessEquipmentRes InsertEquipment(FitnessEquipment fitnessEquipment)
        {
            FitnessEquipmentRes res = new()
            {
                message = "健身器材插入失败！",
                lastUpdateTime = DateTime.Now
            };

            long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond;
            // 使用时间戳创建唯一的 objectName
            string objectName = $"{fitnessEquipment.equipmentName}_{timestamp}.png";
            int base64Index = fitnessEquipment.imgUrl.IndexOf("base64,") + 7;
            fitnessEquipment.imgUrl = fitnessEquipment.imgUrl.Substring(base64Index);
            OSSHelper.UploadBase64ImageToOss(fitnessEquipment.imgUrl, objectName);
            fitnessEquipment.imgUrl = OSSHelper.GetPublicObjectUrl(objectName);

            if (fitnessAIGuideDAL.InsertFitnessEquipment(fitnessEquipment))
                res.message = "健身器材插入成功！";

            return res;

        }

        // 删除单条健身器材记录
        public MessageRes DeleteFitnessEquipment(string equipmentName)
        {
            MessageRes deleteRes = new()
            {
                message = "健身器材删除失败！"
            };

            if (fitnessAIGuideDAL.DeleteFitnessEquipmentByName(equipmentName))
                deleteRes.message = "健身器材删除成功！";

            return deleteRes;
        }

        // 更新健身器材记录
        public MessageRes UpdateFitnessEquipment(FitnessEquipment fitnessEquipment)
        {
            MessageRes res = new()
            {
                message = "健身器材更新失败！"
            };

            if (fitnessAIGuideDAL.UpdateFitnessEquipment(fitnessEquipment))
                res.message = "健身器材更新成功！";

            return res;
        }



        public async IAsyncEnumerable<string> EquipmentMultiAIDialogStream(AIInfo info)
        {
            await foreach (var partialResponse in Qwen_VL.CallQWenMultiStream(info.messages))
            {
                yield return partialResponse;
                await Task.Delay(200);  // 模拟流式输出的延迟
            }
        }


    }
}
