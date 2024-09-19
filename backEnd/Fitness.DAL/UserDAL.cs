using Fitness.DAL.Core;
using Fitness.Models;
using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using System;

namespace Fitness.DAL
{
    public class UserDAL
    {
        private static User ToModel(DataRow row)
        {
            User user = new(row["userName"].ToString(), row["Email"].ToString(), row["Password"].ToString(), row["Salt"].ToString());

            user.userID = Convert.ToInt32(row["UserID"]);
            user.registrationTime = Convert.ToDateTime(row["registrationTime"]);


            user.iconURL = row["iconURL"].ToString();
            user.Age = Convert.ToInt32(row["Age"]);
            user.Gender = row["Gender"].ToString();
            user.Tags = row["Tags"].ToString();
            user.Introduction = row["Introduction"].ToString();

            user.isMember = Convert.ToInt32(row["isMember"]);
            user.isPost = Convert.ToInt32(row["isPost"]);
            user.isDelete = Convert.ToInt32(row["isDelete"]);

            user.goalType = Convert.ToString(row["goalType"]);
            user.goalWeight = Convert.ToSingle(row["goalWeight"]);

            return user;
        }

        private static List<User> ToModelList(DataTable dt)
        {
            List<User> userList = new();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                User user = ToModel(dr);
                userList.Add(user);
            }
            return userList;
        }

        private static LoginInfo LoginInfoToModel(DataRow row)
        {
            LoginInfo logininfo = new(Convert.ToInt32(row["userID"]), row["Password"].ToString(), row["Salt"].ToString());
            return logininfo;
        }
        private static expandUserInfo expandUserInfoToModel(DataRow row)
        {
            expandUserInfo expandinfo = new(Convert.ToInt32(row["userID"]), row["userName"].ToString(), row["iconURL"].ToString(), Convert.ToInt32(row["Age"]), row["Gender"].ToString(), row["Tags"].ToString(), row["Introduction"].ToString(), Convert.ToInt32(row["isMember"]), Convert.ToString(row["goalType"]), Convert.ToSingle(row["goalWeight"]));
            return expandinfo;
        }

        private static basicUserInfo basicUserInfoToModel(DataRow row)
        {
            basicUserInfo basicinfo = new(Convert.ToInt32(row["userID"]), row["userName"].ToString(), row["iconURL"].ToString(), Convert.ToString(row["Email"]),Convert.ToDateTime(row["registrationTime"]), Convert.ToInt32(row["isPost"]), Convert.ToInt32(row["isDelete"]), Convert.ToInt32(row["isMember"]));
            return basicinfo;
        }
        private static List<basicUserInfo> basicUserInfoToModelList(DataTable dt)
        {
            List<basicUserInfo> basic_userInfoList = new();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                basicUserInfo userinfo = basicUserInfoToModel(dr);
                basic_userInfoList.Add(userinfo);
            }
            return basic_userInfoList;
        }

        private static List<expandUserInfo> expandUserInfoToModelList(DataTable dt)
        {
            List<expandUserInfo> ex_userInfoList = new();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                expandUserInfo userinfo = expandUserInfoToModel(dr);
                ex_userInfoList.Add(userinfo);
            }
            return ex_userInfoList;
        }

        public static bool IsEmailRegistered(string email)
        {
            try
            {
                DataTable dt = OracleHelper.ExecuteTable("SELECT * FROM \"User\" WHERE \"Email\"=:Email",
                    new OracleParameter("Email", OracleDbType.Varchar2) { Value = email }
                );

                if (dt.Rows.Count != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public static bool IsEmailInManager(string email)
        {
            try
            {
                DataTable dt = OracleHelper.ExecuteTable("SELECT * FROM \"Manager\" WHERE \"Email\"=:Email",
                    new OracleParameter("Email", OracleDbType.Varchar2) { Value = email }
                );

                if (dt.Rows.Count != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        //获取所有用户
        public static List<basicUserInfo> GetAllUser(out int status)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
               {
               };
                DataTable dt = OracleHelper.ExecuteTable("SELECT \"userID\",\"userName\",\"iconURL\", \"Email\", \"registrationTime\", \"isPost\",\"isDelete\", \"isMember\" FROM \"User\""
                    , oracleParameters);
                if (dt.Rows.Count == 0)
                {
                    status = 2; //数据不存在
                    return null;
                }
                status = 0;//操作成功
                return basicUserInfoToModelList(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;//操作失败
                return null;
            }
        }

        // 根据 UserName 获取 User
        public static List<expandUserInfo> GetUserByUserName(string userName, out int status)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
               {
                    new OracleParameter("\"userName\"", OracleDbType.NVarchar2) {Value = userName},
                    new OracleParameter("isDelete", OracleDbType.Int32) {Value = 0 }

               };
                DataTable dt = OracleHelper.ExecuteTable("SELECT \"userID\",\"userName\", \"iconURL\", \"Age\", \"Gender\",\"Tags\", \"Introduction\",\"isMember\",\"goalType\",\"goalWeight\" FROM \"User\" WHERE \"userName\"=:userName AND \"isDelete\" = :isDelete"
                    , oracleParameters);
                if (dt.Rows.Count == 0)
                {
                    status = 2; //数据不存在
                    return null;
                }
                status = 0;//操作成功
                return expandUserInfoToModelList(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;//操作失败
                return null;
            }
        }

        public static LoginInfo GetLoginInfoByEmail(string email, out int status)
        {
            try
            {
                DataTable dt = OracleHelper.ExecuteTable("SELECT \"userID\", \"Password\",\"Salt\" FROM \"User\" WHERE \"Email\"=:Email",
                    new OracleParameter("Email", OracleDbType.Varchar2) { Value = email });
                if (dt.Rows.Count != 1)
                {
                    status = 2;//邮箱不存在
                    return null;
                }
                status = 0;//查找成功
                DataRow dr = dt.Rows[0];
                return LoginInfoToModel(dr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;//其他错误
                return null;
            }
        }

        // 根据 UserID 获取 User
        public static User GetUserByUserID(int userID, out int status)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
               {
                    new OracleParameter("userID", OracleDbType.Int32) {Value = userID},
                    new OracleParameter("isDelete", OracleDbType.Int32) {Value = 0 }

               };
                DataTable dt = OracleHelper.ExecuteTable("SELECT * FROM \"User\" WHERE \"userID\"=:userID AND \"isDelete\" = :isDelete"
                    , oracleParameters);
                if (dt.Rows.Count != 1)
                {
                    status = 2;
                    return null;
                }
                status = 0;
                DataRow dr = dt.Rows[0];
                return ToModel(dr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;
                return null;
            }
        }

        // 根据 UserID 更新 expandUserInfo
        public static bool UpdateExpandUserInfo(int userID, expandUserInfo expanduserInfo, out int status)
        {
            try
            {
                string iconUrl = expanduserInfo.iconURL;
                if (iconUrl != "null" && !UrlHelper.IsUrl(iconUrl))
                {
                    string object_name = "user" + userID + "_icon" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                    int base64Index = iconUrl.IndexOf("base64,") + 7;
                    iconUrl = iconUrl.Substring(base64Index);
                    OSSHelper.UploadBase64ImageToOss(iconUrl, object_name);
                    iconUrl = OSSHelper.GetPublicObjectUrl(object_name);
                }
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("userName", OracleDbType.NVarchar2) {Value = expanduserInfo.userName},
                    new OracleParameter("iconURL", OracleDbType.Varchar2) {Value =iconUrl},
                    new OracleParameter("Age", OracleDbType.Int32) {Value =expanduserInfo.Age},
                    new OracleParameter("Gender", OracleDbType.Varchar2) {Value = expanduserInfo.Gender},
                    new OracleParameter("Introduction", OracleDbType.NVarchar2) {Value = expanduserInfo.Introduction},
                    new OracleParameter("isMember", OracleDbType.Varchar2) {Value = expanduserInfo.isMember},
                    new OracleParameter("goalType", OracleDbType.NVarchar2) {Value = expanduserInfo.goalType},
                    new OracleParameter("Tags", OracleDbType.NVarchar2) {Value = expanduserInfo.Tags},
                    new OracleParameter("goalWeight", OracleDbType.BinaryFloat) {Value = expanduserInfo.goalWeight},
                    new OracleParameter("userID", OracleDbType.Int32) {Value = userID}
                };
                string sql = "UPDATE \"User\" SET \"userName\"=:userName,\"iconURL\"=:iconURL,\"Age\"=:Age,\"Gender\"=:Gender,\"Introduction\"=:Introduction,\"isMember\"=:isMember,\"goalType\"=:goalType,\"Tags\"=:Tags,\"goalWeight\"=:goalWeight WHERE \"userID\"=:userID";
                int dt = OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                if (dt != 1)
                {
                    status = 2;
                    return false;
                }
                status = 0;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;
                return false;
            }
        }

        // 插入新的 User
        public static int Insert(User user)
        {
            try
            {
                string sql = $"INSERT INTO \"User\"(\"userName\", \"Password\", \"Salt\",\"Email\", \"registrationTime\", \"Age\", \"Gender\", \"isMember\", \"isPost\", \"isDelete\", \"iconURL\", \"Tags\", \"Introduction\",\"goalType\",\"goalWeight\") " +
                             $"VALUES(:\"userName\",:\"Password\", :\"Salt\",:\"Email\", :\"registrationTime\", :\"Age\", :\"Gender\", :\"isMember\", :\"isPost\", :\"isDelete\", :\"iconURL\", :\"Tags\", :\"Introduction\",:\"goalType\",:\"goalWeight\") " +
                             $"RETURNING \"userID\" INTO :userID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"userName\"", OracleDbType.NVarchar2) {Value = user.userName},
                    new OracleParameter("\"Password\"", OracleDbType.Varchar2) {Value = user.Password},
                    new OracleParameter("\"Salt\"", OracleDbType.Varchar2) {Value = user.Salt},
                    new OracleParameter("\"Email\"", OracleDbType.Varchar2) {Value = user.Email},
                    new OracleParameter("\"registrationTime\"", OracleDbType.TimeStamp) {Value = user.registrationTime},
                    new OracleParameter("\"Age\"", OracleDbType.Int32) {Value = user.Age},
                    new OracleParameter("\"Gender\"", OracleDbType.Varchar2) {Value = user.Gender},
                    new OracleParameter("\"isMember\"", OracleDbType.Int32) {Value = user.isMember},
                    new OracleParameter("\"isPost\"", OracleDbType.Int32) {Value = user.isPost},
                    new OracleParameter("isDelete", OracleDbType.Int32) {Value = user.isDelete},
                    new OracleParameter("\"iconURL\"", OracleDbType.Varchar2) {Value = user.iconURL},
                    new OracleParameter("\"Tags\"", OracleDbType.Clob) {Value = user.Tags},
                    new OracleParameter("\"Introduction\"", OracleDbType.NVarchar2) {Value = user.Introduction},
                    new OracleParameter("\"goalType\"", OracleDbType.NVarchar2) {Value = user.goalType},
                    new OracleParameter("\"goalWeight\"", OracleDbType.Single) {Value = user.goalWeight},
                    new OracleParameter("userID", OracleDbType.Int32, ParameterDirection.Output)
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                // 读取 classID 参数的值
                var userIDParam = oracleParameters[15];
                int userID = userIDParam.Value is OracleDecimal oracleDecimal
                    ? oracleDecimal.ToInt32()
                    : Convert.ToInt32(userIDParam.Value);
                return userID;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        // 根据 UserID 删除 User:硬删除
        public static bool DeleteByUserID(int userID)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }
                };
                OracleHelper.ExecuteNonQuery("DELETE FROM \"User\" WHERE \"userID\"=:userID", null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //软删除
        public static bool SetIsDelete(int userID, int isDelete)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("isDelete", OracleDbType.Int32) { Value = isDelete },
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }
                };
                string sql = "UPDATE \"User\" SET \"isDelete\" = :isDelete WHERE \"userID\"=:userID";

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool SetIsPost(int userID, int isPost)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("isPost", OracleDbType.Int32) { Value = isPost },
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }
                };
                string sql = "UPDATE \"User\" SET \"isPost\" = :isPost WHERE \"userID\"=:userID";
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool UpdateLoginTime(int userID, DateTime dateTime)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("lastLoginTime", OracleDbType.TimeStamp) { Value = dateTime },
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }
                };
                string sql = "UPDATE \"User\" SET \"lastLoginTime\" = :LastLoginTime WHERE \"userID\"=:userID";
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static string GetNameById(int userID) {
            OracleParameter[]
                oracleParameters = new OracleParameter[]
                   {
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID},
                    new OracleParameter("isDelete", OracleDbType.Int32) { Value = 0 }
                    };
            DataTable dt = OracleHelper.ExecuteTable("SELECT \"userName\" FROM \"User\" WHERE \"userID\"=:userID AND \"isDelete\" = :isDelete"
                , oracleParameters);
            DataRow dr = dt.Rows[0];
            return Convert.ToString(dr["userName"]);
        }

        public static DateTime GetLastLoginTime(int userID)
        {
            OracleParameter[]
            oracleParameters = new OracleParameter[]
               {
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID},
                    new OracleParameter("isDelete", OracleDbType.Int32) { Value = 0 }
                };
            DataTable dt = OracleHelper.ExecuteTable("SELECT \"lastLoginTime\" FROM \"User\" WHERE \"userID\"=:userID AND \"isDelete\" = :isDelete"
                , oracleParameters);
            DataRow dr = dt.Rows[0];
            return Convert.ToDateTime(dr["lastLoginTime"]);
        }

        public static bool SetRole(int userID, string role) {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("Role", OracleDbType.NVarchar2) { Value = role },
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }
                };
                string sql = "UPDATE \"User\" SET \"Role\" = :Role WHERE \"userID\"=:userID";
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    public static string GetRole(int userID)
        {
            try
            {
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("userID", OracleDbType.Int32) { Value = userID }
                };
                string sql = "SELECT \"Role\" FROM \"User\" WHERE \"userID\"=:userID";
                var dt=OracleHelper.ExecuteTable(sql,oracleParameters);
                return Convert.ToString(dt.Rows[0]["Role"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


    }
}
