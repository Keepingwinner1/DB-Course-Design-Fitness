using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.DAL.Core;
using Newtonsoft.Json;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Fitness.DAL;
using Fitness.BLL;
using Fitness.Models;

namespace Fitness.BLL
{
    public sealed class UserFitnessPlanGoalBLL
    {

        private static readonly UserFitnessPlanGoalBLL instance = new UserFitnessPlanGoalBLL();
        private UserFitnessPlanGoalBLL()
        {
        }
        public static UserFitnessPlanGoalBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public static string Set(int userId, string goal, int duration)
        {
            if (!UserFitnessPlanGoalDAL.Exist(userId))
                UserFitnessPlanGoalDAL.Init(userId);
            UserFitnessPlanGoalDAL.Update(userId, goal, duration);
            UserFitnessPlanDAL.DeletePlan(userId);
            UserFitnessPlanDAL.GeneratePlan(userId, goal);
            return JsonConvert.SerializeObject(new
            {
                isSuccessful = "true"
            });
        }

        public static string Get(int userId)
        {
            return UserFitnessPlanGoalDAL.Get(userId);
        }


    }
}
