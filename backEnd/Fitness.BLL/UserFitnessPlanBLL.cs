using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL
{
    public sealed class UserFitnessPlanBLL
    {
        private static readonly UserFitnessPlanBLL instance = new UserFitnessPlanBLL();
        private UserFitnessPlanBLL()
        {
        }
        public static UserFitnessPlanBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public static void GeneratePlan(string goal)
        {
            
        }
    }
}
