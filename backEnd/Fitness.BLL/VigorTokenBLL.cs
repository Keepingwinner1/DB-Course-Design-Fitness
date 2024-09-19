using Fitness.BLL.Interfaces;
using Fitness.DAL;
using System.Data;
using Fitness.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.Models;
using Fitness.BLL.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Fitness.BLL
{
    public class VigorTokenBLL : IVigorTokenBLL
    {
        VigorTokenDAL vigorTokenDAL = new();

        public BalanceRes GetBalance(int userID)
        {


            DataTable dt = vigorTokenDAL.GetVigorTokenBalance(userID);
            
            DataRow dr = dt.Rows[0];
            
            BalanceRes res = new()
            {
                balance=0 
            };

            res.balance = Convert.ToInt32(dr["vigorTokenBalance"]);
            Console.WriteLine($"balance{res.balance}");
            return res;
        }

        public MessageRes UpdateBalance(int userID, string reason, int change)
        {

            int balance = GetBalance(userID).balance;


            VigorTokenInfo vigorTokenInfo = new()
            {
                userID = userID,

                reason = reason,

                change = change,

                balancce = balance + change,

                createTime = DateTime.Now

            };

            MessageRes res = new()
            {
                message = "更新活力币余额失败!"
            };

            if (vigorTokenDAL.UpdateVigorTokenBalance(change, userID))
            {
                if (vigorTokenDAL.InsertVigorTokenRecord(vigorTokenInfo))
                {
                    res.message = "更新活力币余额成功！";
                }
            }

            return res;
        }

        // 获取所有的活力币变化记录
        public VigorTokenRecordList GetAllVigorTokenRecords(int userID)
        {


            DataTable dt = vigorTokenDAL.GetVigorTokenRecordsByUsrID(userID);
            VigorTokenRecordList res = new();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                VigorTokenRecordRes single = new();
                DataRow dr = dt.Rows[i];
                single.recordID = Convert.ToInt32(dr["recordID"]);
                single.reason = dr["reason"].ToString();
                single.change = Convert.ToInt32(dr["change"]);
                single.balance = Convert.ToInt32(dr["balance"]);
                single.createTime = Convert.ToDateTime(dr["createTime"]);
                res.records.Add(single);

            }
            return res;
        }



    }
}
