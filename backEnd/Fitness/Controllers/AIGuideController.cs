using Fitness.BLL;
using Fitness.BLL.Core;
using Fitness.BLL.Interfaces;
using Fitness.Models;
using Markdig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AIGuideController : ControllerBase
    {
        private readonly IFitnessALGuideBLL _fitnessALGuideBLL;
        private readonly JWTHelper _jwtHelper = new();

        public AIGuideController(IFitnessALGuideBLL fitnessALGuideBLL)
        {
            _fitnessALGuideBLL = fitnessALGuideBLL;

        }

        // 上传健身截图创建记录
        [HttpPost]
        public ActionResult<ScreenshotRes> Create(string token,CreateScreenshot screenshot)
        {
            Console.WriteLine($"健身截图创建");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            ScreenshotInfo screenshotInfo = new()
            {
                userID = userID,
                screenshotUrl = screenshot.screenshotUrl,
                exerciseName = screenshot.exerciseName
            };
            return _fitnessALGuideBLL.Create(screenshotInfo);
        }


        // 获取所有健身截图记录
        [HttpGet]
        public ActionResult<SuggestionDetailRes> GetAllDetails(string token)
        {
            Console.WriteLine($"获取所有健身截图记录");
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            return _fitnessALGuideBLL.GetAllDetails(userID);
        }


        // 获取AI对健身截图的建议
        [HttpGet]
        public ActionResult<MessageRes> GetAISuggestion(int screenshotID)
        {
            Console.WriteLine($"获取AI建议{screenshotID}");
            return _fitnessALGuideBLL.GetAISuggestion(screenshotID);
        }


        // 根据screenshotID删除单条健身动作记录
        [HttpDelete]
        public ActionResult<MessageRes> Delete(int screenshotID)
        {
            Console.WriteLine($"删除健身动作截图记录{screenshotID}");
            return _fitnessALGuideBLL.DeleteFitnessSuggestion(screenshotID);
        }

        // 获取健身器材使用方法
        [HttpGet]
        public ActionResult<SingleDetailEqui> GetEquipmentGuide(string equipmentName)
        {
            Console.WriteLine($"获取健身器材详情{equipmentName}");
            return _fitnessALGuideBLL.GetOneEquiGuide(equipmentName);
        }

        // 获取所有健身器材的缩略使用方法
        [HttpGet]
        public ActionResult<AllEquiGuidesRes> GetALLEquipmentGuide()
        {
            Console.WriteLine($"获取所有健身器材的信息");
            return _fitnessALGuideBLL.GetALLEquiGuides();
        }

        // 获取所有健身器材的缩略使用方法
        [HttpGet]
        public ActionResult<RandomEquiGuidesRes> GetRandomEquipmentGuide()
        {
            Console.WriteLine($"随机获取7种健身器材的缩略信息");
            return _fitnessALGuideBLL.GetRandomEquiGuides();
        }
        
        // 增
        [HttpPost]
        public ActionResult<FitnessEquipmentRes> InsertEquipmentGuide(FitnessEquipment fitnessEquipment)
        {
            Console.WriteLine($"插入健身器材记录{fitnessEquipment.equipmentName}");
            return _fitnessALGuideBLL.InsertEquipment(fitnessEquipment);
        }

        // 删
        [HttpDelete]
        public ActionResult<MessageRes> DeleteEquipmentGuide(string equipmentName)
        {
            Console.WriteLine($"删除健身器材记录{equipmentName}");
            return _fitnessALGuideBLL.DeleteFitnessEquipment(equipmentName);
        }

        // 改
        [HttpPut]
        public ActionResult<MessageRes> UpdateEquipmentGuide(FitnessEquipment fitnessEquipment)
        {
            Console.WriteLine($"更新健身器材记录{fitnessEquipment.equipmentName}");
            return _fitnessALGuideBLL.UpdateFitnessEquipment(fitnessEquipment);
        }



        // 修改后的AI智能问答API
        [HttpGet]
        public async Task LLM(string equipmentName,string messages)
        {
            Console.WriteLine($"AI问答{equipmentName}");

            // 解析messages为List<AIMessage>
            var messageList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AIMessage>>(messages);

            // 设置响应头，开启流式传输
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            AIMessage message = new()
            {
                role = "system",
                content = $"- Role: 资深AI健身教练\r\n" +
                $"- Background: 用户希望了解{equipmentName}这一健身器材，但对其使用方法和注意事项不够了解，需要专业的指导和建议。\r\n" +
                $"- Profile: 你是一位经验丰富的AI健身教练，专注于{equipmentName}训练，对其的使用技巧和安全事项有深入的了解。\r\n" +
                $"- Skills: 你具备人体运动学、运动生理学和运动心理学的知识，能够根据用户的身体状况和健身目标，提供个性化的{equipmentName}训练计划。\r\n" +
                $"- Goals: 为用户提供安全、有效的{equipmentName}训练指导，帮助用户达到健身目标，同时避免运动伤害。\r\n" +
                $"- Constrains: 确保所有指导和建议都基于科学原理和安全标准，适合不同年龄和体能水平的用户。\r\n" +
                $"- OutputFormat: 提供文字说明和互动问答。输出绝对不要用#号来区分大小标题，字号都一样就行了\r\n" +
                $"- Workflow:\r\n  " +
                $"1. 了解用户的身体状况和健身目标。\r\n  " +
                $"2. 根据用户情况，制定个性化的{equipmentName}训练计划。\r\n  " +
                $"3. 教授{equipmentName}的正确使用方法和注意事项。\r\n  " +
                $"4. 提供{equipmentName}训练的安全指南。\r\n  " +
                $"5. 根据用户反馈，调整训练计划和方法。"
            };

            AIInfo info = new()
            {
                equipmentName = equipmentName,
                messages = messageList
            };

            info.messages.Insert(0, message);

            // 调用多次生成AI的响应内容
            await foreach (var chunk in _fitnessALGuideBLL.EquipmentMultiAIDialogStream(info))
            {
                

                Console.WriteLine($"Sending chunk: {chunk}");
                await Response.WriteAsync($"data:{chunk}\n\n");
                await Response.Body.FlushAsync();  // 确保每个块立即发送到前端
            }

            Console.WriteLine("Sending [DONE]");
            await Response.WriteAsync("data:[DONE]\n\n");
            await Response.Body.FlushAsync();


        }


    }
}
