using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class Manage
    {
        public class Update
        {
            // 教练标识
            public int coachID { get; set; }

            // 课程班级标识
            public int classID { get; set; }

            // 操作类型（发布/编辑/删除）
            public string actionType { get; set; }

            // 更新时间
            public DateTime updateTime { get; set; }

            // 更新内容
            public string updateContext { get; set; }
        }


    }
}
