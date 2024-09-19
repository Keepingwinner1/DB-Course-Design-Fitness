using Fitness.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IFitnessBLL
    {
        public string Update(int userId, double height, double weight, double BMI, double bodyFatRate);

        public string Get(int userId);
    }
}
