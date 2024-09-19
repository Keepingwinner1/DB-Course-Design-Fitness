using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Sts.Model.V20150401;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Fitness.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StsController : ControllerBase
    {
        private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "keys.xml");
        private static XDocument xDoc = XDocument.Load(filePath);
        private readonly string accessKeyId = xDoc.Root.Element("accessKeyId").Value;
        private readonly string accessKeySecret = xDoc.Root.Element("accessKeySecret").Value;
        private readonly string roleArn = xDoc.Root.Element("roleArn").Value;
        private readonly string regionId = xDoc.Root.Element("regionId").Value;

        [HttpGet("GetStsToken")]
        public IActionResult GetStsToken()
        {
            try
            {
                IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, accessKeySecret);
                DefaultAcsClient client = new DefaultAcsClient(profile);

                AssumeRoleRequest request = new AssumeRoleRequest
                {
                    RoleArn = roleArn,
                    RoleSessionName = "upload-session"
                };

                AssumeRoleResponse response = client.GetAcsResponse(request);

                return Ok(new
                {
                    AccessKeyId = response.Credentials.AccessKeyId,
                    AccessKeySecret = response.Credentials.AccessKeySecret,
                    SecurityToken = response.Credentials.SecurityToken,
                    Expiration = response.Credentials.Expiration
                });
            }
            catch (Exception ex)
            {
                return BadRequest("获取 STS 临时凭证失败: " + ex.Message);
            }
        }
    }
}

