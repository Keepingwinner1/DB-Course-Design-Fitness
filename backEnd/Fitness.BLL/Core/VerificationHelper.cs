using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Core
{
    public class VerificationHelper
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public VerificationHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public void StoreVerificationCode(string email, string code)
        {
            _memoryCache.Set(email, code, _cacheDuration);
        }

        public bool ValidateVerificationCode(string email, string code)
        {
            if (_memoryCache.TryGetValue(email, out string storedCode))
            {
                return storedCode == code;
            }
            return false;
        }

        public void RemoveVerificationCode(string email)
        {
            _memoryCache.Remove(email);
        }

        public void SendVerificationEmail(string email, string code)
        {
            var fromAddress = new MailAddress("2116302797@qq.com", "FitFitFromCHL");
            var toAddress = new MailAddress(email);
            const string fromPassword = "spiusyacrwvycjbb";
            const string subject = "FitFit注册验证码";
            string body = $"感谢你注册FitFit,你的验证码是 {code}";

            var smtp = new SmtpClient
            {
                Host = "smtp.qq.com", // 你的SMTP服务器地址
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
