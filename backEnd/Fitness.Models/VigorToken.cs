using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class VigorTokenInfo
    {
        public int userID { get; set; }

        public string reason { get; set; }

        public int change { get; set; }

        public int balancce { get; set; }

        public DateTime createTime { get; set; }

    }

    public class BalanceRes
    {
        public int balance { get; set; }
    }

    public class VigorTokenRecordRes
    {
        public int recordID { get; set; }

        public string reason { get; set; }

        public int change { get; set; }

        public int balance { get; set; }

        public DateTime createTime { get; set; }
    }

    public class VigorTokenRecordList
    {
        public List<VigorTokenRecordRes> records { get; set; } = new();
    }


}
