using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class Friendship
    {
        public int friendshipID { get; set; }
        public int userID { get; set; }
        public int friendID { get; set; }
        public DateTime createdtime { get; set; }
    }

}
