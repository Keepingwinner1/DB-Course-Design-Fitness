using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.DAL;
using Fitness.BLL;
using Fitness.Models;
using Fitness.BLL.Interfaces;

namespace Fitness.BLL
{
    public sealed class FitnessBLL
    {
        private static readonly FitnessBLL instance = new FitnessBLL();
        private FitnessBLL()
        {
        }
        public static FitnessBLL Instance
        {
            get
            {
                return instance;
            }
        }

        public static string Update(int userId, double height, double weight, double BMI, double bodyFatRate)
        {
            string s = FitnessDAL.Update(userId, height, weight, BMI, bodyFatRate);
            return JsonConvert.SerializeObject(new
            {
                message = s
            });
        }

        public static string Get(int userId)
        {
            return FitnessDAL.Get(userId);
        }
    }
}
