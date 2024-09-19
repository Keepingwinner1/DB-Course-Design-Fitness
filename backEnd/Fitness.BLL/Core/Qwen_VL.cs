using Fitness.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fitness.BLL.Core
{
    public class Qwen_VL
    {
        private static readonly HttpClient client = new();

        

        private static readonly string apiUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/multimodal-generation/generation";
        static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "keys.xml");
        static XDocument xDoc = XDocument.Load(filePath);
        // 私有静态只读字段，存储数据库连接字符串
        static readonly string apiKey = xDoc.Root.Element("qwenApiKey").Value;

        public static async Task<string> CallQWenVL(string prompt, string imgUrl = null)
        {


            var requestBody = new
            {
                model = "qwen-vl-plus",
                input = new
                {
                    messages = new[]
                        {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { image = imgUrl },
                                new { text = prompt }
                            }
                        }
                    }
                },
                parameters = new { }
            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);

                // 解析返回的JSON字符串
                var jsonResponse = JObject.Parse(responseString);
                var contentResult = jsonResponse["output"]["choices"][0]["message"]["content"][0]["text"].ToString();
                return contentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求失败: {ex.Message}");
                return null;
            }
        }


        public static string CallQWenSingle(string sys, string prompt)
        {
            var requestBody = new
            {
                model = "qwen-plus",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = sys
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },

            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");


            string NonVLAPIUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";


            var request = new HttpRequestMessage(HttpMethod.Post, NonVLAPIUrl)
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = client.SendAsync(request).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);

                // 解析返回的JSON字符串
                var jsonResponse = JObject.Parse(responseString);
                var contentResult = jsonResponse["choices"][0]["message"]["content"].ToString();
                return contentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求失败: {ex.Message}");
                return null;
            }
        }

        public static string CallQWenMulti(List<AIMessage> messages)
        {
            var requestBody = new
            {
                model = "qwen-plus",
                messages,
                stream = true

            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");


            string NonVLAPIUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";


            var request = new HttpRequestMessage(HttpMethod.Post, NonVLAPIUrl)
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = client.SendAsync(request).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);

                // 解析返回的JSON字符串
                var jsonResponse = JObject.Parse(responseString);
                var contentResult = jsonResponse["choices"][0]["message"]["content"].ToString();
                return contentResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求失败: {ex.Message}");
                return null;
            }
        }

        public static async IAsyncEnumerable<string> CallQWenMultiStream(List<AIMessage> messages)
        {
            async IAsyncEnumerable<string> ReadStreamAsync(StreamReader reader)
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("data:"))
                    {
                        string jsonString = line.Substring("data:".Length).Trim();
                        if (jsonString == "[DONE]")
                        {
                            break; // 完成流处理
                        }

                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            var jsonResponse = JObject.Parse(jsonString);
                            var deltaContent = jsonResponse["choices"][0]["delta"]["content"].ToString();

                            if (!string.IsNullOrEmpty(deltaContent))
                            {
                                // 替换换行符为标记符
                                deltaContent = deltaContent.Replace("\n", "xx ");
                                yield return deltaContent;
                            }
                        }
                    }
                }
            }


            var requestBody = new
            {
                model = "qwen-plus",
                messages,
                stream = true
            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            string NonVLAPIUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";

            var request = new HttpRequestMessage(HttpMethod.Post, NonVLAPIUrl)
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode(); // 确保请求成功
            var stream = await response.Content.ReadAsStreamAsync();

            // 第三个步骤添加的位置：处理流式传输的结束情况
            string accumulatedContent = string.Empty;
            using (var reader = new StreamReader(stream))
            {
                await foreach (var deltaContent in ReadStreamAsync(reader))
                {
                    accumulatedContent += deltaContent;
                    yield return accumulatedContent;
                    accumulatedContent = string.Empty; // 清空缓冲区
                }

                // 确保最后剩余的内容被发送
                if (!string.IsNullOrEmpty(accumulatedContent))
                {
                    yield return accumulatedContent;
                }
            }
        }
    }

}
