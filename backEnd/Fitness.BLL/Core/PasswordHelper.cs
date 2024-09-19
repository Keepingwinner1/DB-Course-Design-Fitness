using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Core
{
    public class PasswordHelper
    {
        //基于 PBKDF2（Password-Based Key Derivation Function 2）的哈希算法
        //PBKDF2 是一种密码哈希算法，它利用一个密码、一个盐值和一个迭代次数来生成一个密钥
        public static string HashPassword(string password, out string salt)
        {
            // 生成随机盐值
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // 生成哈希值
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            byte[] hashBytes = pbkdf2.GetBytes(20);

            // 合并盐值和哈希值
            byte[] hashWithSaltBytes = new byte[36];
            Array.Copy(saltBytes, 0, hashWithSaltBytes, 0, 16);
            Array.Copy(hashBytes, 0, hashWithSaltBytes, 16, 20);

            return Convert.ToBase64String(hashWithSaltBytes);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            // 生成哈希值
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            byte[] enteredHashBytes = pbkdf2.GetBytes(20);

            // 比较哈希值
            for (int i = 0; i < 20; i++)
            {
                if (storedHashBytes[i + 16] != enteredHashBytes[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
