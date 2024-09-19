using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class UserAchievement
    {
        public int UserID { get; set; }
        public int AchievementID { get; set; }
        public int Progress { get; set; }
        public string? AchievedDate { get; set; }
        public int IsAchieved { get; set; }
    }
}
