using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fitness.BLL.Interfaces;
using Fitness.Models;
using Fitness.BLL;
using Fitness.BLL.Core;

namespace Fitness.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IVigorTokenBLL _vigorTokenBLL;
        private readonly IUserBLL _userBLL;
        private readonly JWTHelper _jwtHelper = new();

        public UserController(IUserBLL userBLL, IVigorTokenBLL vigorTokenBLL)
        {
            _userBLL = userBLL;
            _vigorTokenBLL = vigorTokenBLL;
        }

        [HttpGet]
        public ActionResult<LoginToken> Login(string email, string password, string role)
        {
            return _userBLL.Login(email, password, role);
        }
        [HttpGet]
        public ActionResult<bool> GetTokenInvalidateRes(string token)
        {
            return _jwtHelper.ValidateToken(token).IsValid;
        }

        [HttpPost]
        public ActionResult<string> SendVerificationCode(string email)
        {
            return _userBLL.SendVerificationCode(email);
        }
        [HttpPost]
        public ActionResult<string> Register(RegisterInfo registerInfo)
        {
            return _userBLL.Register(registerInfo);
        }
        [HttpGet]
        public ActionResult<string> RefreshToken(string oldToken)
        {
            return _userBLL.RefreshToken(oldToken);
        }

        [HttpGet]
        public ActionResult<User> GetPersonalProfile(string token)
        {
            return _userBLL.GetProfile(token, "personal");
        }

        [HttpGet]
        public ActionResult<User> GetProfile(string token, int userID)
        {
            return _userBLL.GetProfile(token, "others", userID);
        }
        [HttpGet]
        public ActionResult<string> GetName(int userID) {
            return _userBLL.GetName(userID);
        }
        
        [HttpPost]
        public ActionResult<string> UpdateProfile(string token, [FromBody] expandUserInfo userinfo)
        {
            return _userBLL.UpdateProfile(token, userinfo);//前端不要修改userID字段，保持不变传回即可
        }
        [HttpGet]
        public ActionResult<List<expandUserInfo>> SearchUserByName(string token, string username)
        {
            return _userBLL.GetProfileByName(token, username);
        }

        [HttpGet]
        public ActionResult<List<basicUserInfo>> GetAllUser(string token)
        {
            return _userBLL.GetAllUser(token);
        }

        [HttpGet]
        public ActionResult<expandUserInfo> GetProfileByUserID(string token, int userID)
        {
            return _userBLL.GetProfileByUserID(token, userID);
        }

        private FriendshipBLL _friendshipBll = new FriendshipBLL();

        [HttpGet]
        public ActionResult<List<int>> GetFriendList(string token)
        {
            List<int> friendList = _friendshipBll.GetFriendList(token);
            return Ok(friendList);
        }

        [HttpGet]
        public ActionResult AddFriend(string token, int friendID)
        {
            bool success = _friendshipBll.AddFriend(token, friendID);
            if (success)
            {
                return Ok("添加好友成功");
            }
            else
            {
                return BadRequest("添加好友失败");
            }
        }

        //权限管理
        [HttpGet]
        public ActionResult<string> RemoveUser(string token, int userID)
        {
            return _userBLL.removeUser(token, userID);
        }
        [HttpGet]
        public ActionResult<string> BanUser(string token, int userID)
        {
            return _userBLL.banPost(token, userID);
        }

        [HttpGet]
        public ActionResult<string> CancelBanUser(string token, int userID)
        {
            return _userBLL.CancelbanPost(token, userID);
        }

        // zwc 
        [HttpGet]
        public ActionResult<BalanceRes> GetVigorTokenBalance(string token,int userID)
        {

            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userIDToken = tokenRes.userID;
            if (userID == -1)
                userID = userIDToken;
            Console.WriteLine($"获取活力币余额{userID}");
            return _vigorTokenBLL.GetBalance(userID);
        }

        [HttpGet]
        public ActionResult<VigorTokenRecordList> GetVigorTokenReacords(string token,int userID)
        {
            
            Console.WriteLine("获取活力币记录");
            return _vigorTokenBLL.GetAllVigorTokenRecords(userID);
        }
    }
}
