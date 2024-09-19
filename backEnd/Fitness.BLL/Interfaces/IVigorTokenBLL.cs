using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Fitness.BLL.Interfaces
{
    public interface IVigorTokenBLL
    {
        public BalanceRes GetBalance(int userID);

        public MessageRes UpdateBalance(int userID, string reason, int change);

        public VigorTokenRecordList GetAllVigorTokenRecords(int userID);

    }
}
