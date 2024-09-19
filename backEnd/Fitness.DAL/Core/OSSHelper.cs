using Aliyun.OSS;
using Aliyun.OSS.Common;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fitness.DAL.Core
{
    public static class OSSHelper
    {
        static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "keys.xml");
        static XDocument xDoc = XDocument.Load(filePath);

        static string accessKeyId = xDoc.Root.Element("accessKeyId").Value;
        static string accessKeySecret = xDoc.Root.Element("accessKeySecret").Value;
        static string endpoint = xDoc.Root.Element("endpoint").Value;
        static string bucketName = xDoc.Root.Element("bucketName").Value;    //OSS图片存储空间

        // 该方法用于将Base64编码的图片数据保存为本地文件，并上传到阿里云
        public static bool UploadBase64ImageToOss(string base64Image, string objectName)
        {
            // 创建临时文件名
            string tempFileName = Path.GetTempFileName();
            try
            {
                // 将Base64编码的数据转换为字节数组
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                // 将字节数组写入到本地文件
                File.WriteAllBytes(tempFileName, imageBytes);

                // 调用PutObjectFromLocalFile方法上传文件到阿里云
                return PutObjectFromLocalFile(objectName, tempFileName);
            }
            finally
            {
                // 删除临时文件
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }

        public static bool PutObjectFromLocalFile(string objectName, string localFilename)
        {
            OssClient client = new(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var obj = client.PutObject(bucketName, objectName, localFilename);
                if (obj != null && obj.HttpStatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine(string.Format("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3},BucketName:{4},fileName:{5}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId, bucketName, objectName));
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed with error info: {0},BucketName:{1},fileName:{2}", ex.Message, bucketName, objectName));
                return false;
            }
        }

        public static string GetPublicObjectUrl(string objectName)
        {
            return $"https://{bucketName}.{endpoint}/{objectName}";
        }
    }
}
