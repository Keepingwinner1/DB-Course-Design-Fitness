using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.BLL.Core;

using Fitness.DAL;
using Fitness.Models;

namespace Fitness.BLL
{
    public class FriendshipBLL
    {
        private readonly JWTHelper _jwtHelper = new();
        public List<int> GetFriendList(string token)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            return FriendshipDAL.GetFriendList(userID);
        }

        public bool AddFriend(string token, int friendID)
        {
            TokenValidationResult tokenRes = _jwtHelper.ValidateToken(token);
            int userID = tokenRes.userID;
            // 可以在这里添加更多的业务逻辑，例如检查是否已经是好友
            return FriendshipDAL.AddFriend(userID, friendID);
        }

    }
}
