using Fitness.Models;
using Fitness.BLL.Interfaces;
using Fitness.DAL;
using Fitness.BLL.Core;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using Newtonsoft.Json;
using System.Net;

namespace Fitness.BLL
{
        public class UserBLL : IUserBLL
        {
            private readonly VerificationHelper _verificationHelper;

            public UserBLL(VerificationHelper verificationHelper)
            {
                _verificationHelper = verificationHelper;
            }
            public string Register(RegisterInfo registerInfo)
            {
                int status;
                var flag = UserDAL.IsEmailRegistered(registerInfo.email);

                if (flag) return JsonConvert.SerializeObject(new
                {
                    message = "注册失败：邮箱已存在"
                });

                if (!_verificationHelper.ValidateVerificationCode(registerInfo.email, registerInfo.verificationCode))
                {
                    return JsonConvert.SerializeObject(new
                    {
                        message = "验证码错误或已过期"
                    });
                }
                registerInfo.coachName = "null";
                // 对密码进行加密
                string salt;
                string hashedPassword = PasswordHelper.HashPassword(registerInfo.password, out salt);
                //先统一向User表中插入一条
                User user = new User(registerInfo.accountName, registerInfo.email, hashedPassword, salt);
                int res = UserDAL.Insert(user);
                if (res == -1)
                    return JsonConvert.SerializeObject(new
                    {
                        message = "注册失败：插入User表失败！"
                    });
                if (registerInfo.role == "coach")
                {
                    Coach coach = new Coach(res, user.userName, user.Age, user.Gender, user.iconURL, user.isMember, registerInfo.coachName, "");
                    CoachDAL.Insert(coach);
                }
                _verificationHelper.RemoveVerificationCode(registerInfo.email);
            int st;
            var vigorTokenBLL = new VigorTokenBLL();
            vigorTokenBLL.UpdateBalance(res, "注册FitFit成功，获得2000活力币", 2000);
            //初始化身份与登录时间
            UserDAL.UpdateLoginTime(res, DateTime.Now);
            UserDAL.SetRole(res, registerInfo.role);
            //成就系统、计划系统初始化
            UserAchievementBLL userAchievementBLL = new UserAchievementBLL();
            userAchievementBLL.Init(res); FitnessDAL.Init(res); PhysicalTestDAL.Init(res); UserFitnessPlanGoalDAL.Init(res);
                return JsonConvert.SerializeObject(new
                {
                    message = "成功注册"
                });
            }
            public string SendVerificationCode(string email)
            {
                var code = _verificationHelper.GenerateVerificationCode();
                _verificationHelper.StoreVerificationCode(email, code);

                // 调用发送邮件的逻辑
                _verificationHelper.SendVerificationEmail(email, code);

                return JsonConvert.SerializeObject(new
                {
                    message = "验证码已发送"
                });
            }

            private readonly JWTHelper _jwtHelper = new();
            public LoginToken Login(string email, string password, string role)
            {
                int status;
                LoginInfo loginInfo = UserDAL.GetLoginInfoByEmail(email, out status);
                if (status == 2) { return new LoginToken("InvalidToken", "邮箱不存在或错误"); }
                if (status == 1) return new LoginToken("Invalid", "Internal server error. Please try again later.");
                bool isPass = PasswordHelper.VerifyPassword(password, loginInfo.hashedPassword, loginInfo.Salt);
            if (isPass) {
                //管理员与用户身份校验
                bool isAdmin = false;
                isAdmin = UserDAL.IsEmailInManager(email);
                if (role == "admin" && !isAdmin) { 
                    return new LoginToken("Invalid", "身份权限不符");
                }
                var res = new LoginToken(_jwtHelper.GenerateToken(loginInfo.userID, role), "登录成功");
                int userID = _jwtHelper.ValidateToken(res.token).userID;
                bool isCoach = false;
                isCoach = CoachDAL.IsIDInCoach(userID);
                if (role == "coach" && !isCoach)
                {
                    return new LoginToken("Invalid", "身份权限不符");
                }
                DateTime dt_last = UserDAL.GetLastLoginTime(userID);
                DateTime dt_now = DateTime.Now;
                UserDAL.UpdateLoginTime(userID,dt_now);
                //判断是否首次登录系统
                bool isSameDate = dt_last.Date == dt_now.Date;
                if (!isSameDate)
                {
                    var vigorTokenBLL = new VigorTokenBLL();
                    var userAchievementBLL = new UserAchievementBLL();
                    //活力币+1
                    vigorTokenBLL.UpdateBalance(userID, $"本日{dt_now}首次登录系统,获得50活力币", 50);
                    //成就更新
                    userAchievementBLL.UpdateLoginAchievement(userID);
                }
                return res;
            }
                return new LoginToken("InvalidToken", "密码错误");
            }

            public string RefreshToken(string oldToken)
            {
                string newToken = null;
                bool isValid;

                // 验证现有的 JWT
                TokenValidationResult validateRes = _jwtHelper.ValidateToken(oldToken);
                if (validateRes.IsValid == true)
                {
                    // 从现有 JWT 中提取用户信息
                    var userId = validateRes.userID;
                    var role = validateRes.Role;

                    // 生成新的 JWT
                    newToken = _jwtHelper.GenerateToken(userId, role);
                    return newToken;
                }
                return "InValid";
            }

            public User GetProfile(string token, string role, int userID = -1)
            {
                int st;
                //可以添加一些出错的处理：方便前后端对接出错时处理
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                if (role == "personal")
                    return UserDAL.GetUserByUserID(tokenRes.userID, out st);
                else// "others"
                {
                    return UserDAL.GetUserByUserID(userID, out st);
                }
            }

            public string UpdateProfile(string token, expandUserInfo userinfo)
            {

                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                int st;
                int userid = tokenRes.userID;
                //可以再利用st添加一些出错的处理：方便前后端对接出错时处理
                UserDAL.UpdateExpandUserInfo(userid, userinfo, out st);
                //！！！记得同步更新FoodPlan表
                //补充个人资料更新的成就
                UserAchievementBLL UserAchievementBll = new();
                UserAchievementBll.UpdateUserInfoAchievement(userid);
                return "更新成功";
            }

            public List<expandUserInfo> GetProfileByName(string token, string searchname)
            {
                int st;
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                //返回一些基本的信息：ID,头像，昵称，年龄，性别，粉丝数 
                //！！！补充：未查到的信息返回
                var res = UserDAL.GetUserByUserName(searchname, out st);
                if (st == 2)
                    return null;
                else return res;
            }

        public string GetName(int userID) {
            return UserDAL.GetNameById(userID);
        
        }

            public expandUserInfo GetProfileByUserID(string token, int userID)
            {
                int st;
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                //！！！补充：未查到的信息返回
                var res = UserDAL.GetUserByUserID(userID, out st);
                var result = new expandUserInfo(userID, res.userName, res.iconURL, res.Age, res.Gender, res.Tags, res.Introduction, res.isMember, res.goalType, res.goalWeight);
                if (st == 2)
                    return null;
                else return result;
            }

            public List<basicUserInfo> GetAllUser(string token)
            {
                int st;
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                //返回一些基本的信息：ID,头像，昵称，年龄，性别，粉丝数 
                //！！！补充：未查到的信息返回
                var res = UserDAL.GetAllUser(out st);
                if (st == 2)
                    return null;
                else return res;
            }

            //权限管理
            public string removeUser(string token, int userID)
            {
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                if (tokenRes.Role == "admin")
                {
                    UserDAL.SetIsDelete(userID, 1);
                    return "删除成功";
                }
                else
                    return "身份权限不符";
            }

            public string banPost(string token, int userID) //可以设置禁止发帖的天数
            {
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                if (tokenRes.Role == "admin")
                {
                    UserDAL.SetIsPost(userID, 0);
                    return "禁言成功";
                }
                else
                    return "身份权限不符";
            }

            public string CancelbanPost(string token, int userID) //可以设置禁止发帖的天数
            {
                TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
                if (tokenRes.Role == "admin")
                {
                    UserDAL.SetIsPost(userID, 1);
                    return "取消禁言成功";
                }
                else
                    return "身份权限不符";
            }
    }
    }
