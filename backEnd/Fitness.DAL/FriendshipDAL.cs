using Fitness.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class FriendshipDAL
    {
        public static List<int> GetFriendList(int userID)
        {
            string query = "SELECT \"friendID\" FROM \"Friendship\" WHERE \"userID\" = :userID UNION SELECT \"userID\" FROM \"Friendship\" WHERE \"friendID\" = :userID";
            OracleParameter[] parameters = {
            new OracleParameter("userID", userID)
        };

            DataTable resultTable = OracleHelper.ExecuteTable(query, parameters);
            List<int> friendList = new List<int>();
            foreach (DataRow row in resultTable.Rows)
            {
                friendList.Add(Convert.ToInt32(row["friendID"]));
            }
            return friendList;
        }

        public static bool AddFriend(int userID, int friendID)
        {
            string query = "INSERT INTO \"Friendship\" (\"userID\", \"friendID\") VALUES (:UserID, :FriendID)";
            OracleParameter[] parameters = {
            new OracleParameter("userID", userID),
            new OracleParameter("friendID", friendID)
        };
            int result = OracleHelper.ExecuteNonQuery(query,null, parameters);
            return result > 0;
        }
    }
}
