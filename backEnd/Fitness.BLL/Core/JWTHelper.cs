using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using TokenValidationResult = Fitness.Models.TokenValidationResult;

namespace Fitness.BLL.Core
{
    public class JWTHelper
    {
        //生成Token 验证Token 
        private readonly string _secretKey = "ThisDBcoureseDesign2024ByChenghongeli";
        private readonly string _issuer="FitFitProjectTeam";
        private readonly string _audience= "FitnessEnthusiasts";

        public string GenerateToken(int userId, string role, List<Claim> additionalClaims = null)
        {
            // 创建声明列表
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // 用户ID
                new Claim(ClaimTypes.Role, role), // 角色
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // 签发时间（Unix 时间戳）
            };

            // 添加额外的声明（如果有的话）
            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            // 创建签名密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 创建 JWT token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // 设置过期时间
                signingCredentials: creds);

            // 将 JWT token 转换为字符串
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // 验证 JWT
        public TokenValidationResult ValidateToken(string token)
        {
            // 启用 PII 显示
            IdentityModelEventSource.ShowPII = true;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                // 检查密钥、发行者和受众是否为空
                if (string.IsNullOrEmpty(_secretKey))
                    throw new ArgumentNullException(nameof(_secretKey), "Secret key is null or empty.");
                if (string.IsNullOrEmpty(_issuer))
                    throw new ArgumentNullException(nameof(_issuer), "Issuer is null or empty.");
                if (string.IsNullOrEmpty(_audience))
                    throw new ArgumentNullException(nameof(_audience), "Audience is null or empty.");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // 验证过期时间
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                Console.WriteLine("Token validation succeeded.");
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"Role: {role}, User ID: {userId}");

                return new TokenValidationResult
                {
                    Principal = principal,
                    Role = role,
                    userID = Convert.ToInt32(userId),
                    IsValid = true
                };
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Validation parameter is null: " + ex.Message);
                return new TokenValidationResult
                {
                    IsValid = false
                };
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine("Token expired: " + ex.Message);
                return new TokenValidationResult
                {
                    IsValid = false
                };
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine("Token validation failed: " + ex.Message);
                return new TokenValidationResult
                {
                    IsValid = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                return new TokenValidationResult
                {
                    IsValid = false
                };
            }
        }

    }
}
