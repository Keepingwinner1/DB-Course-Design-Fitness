using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.DAL.Core;
using Newtonsoft.Json;
using System.Data;
using Aliyun.OSS;
using Fitness.DAL;
using Fitness.Models;

namespace Fitness.DAL
{
    public sealed class ExerciseDAL
    {
        private static readonly ExerciseDAL instance = new ExerciseDAL();
        private ExerciseDAL()
        {
        }
        public static ExerciseDAL Instance
        {
            get
            {
                return instance;
            }
        }

        public static Exercise Get(string exerciseName)
        {
            string query = "SELECT \"exerciseName\", \"explanation\", \"equipmentName\", \"part\", \"gifUrl\", \"time\", \"count\", \"coverUrl\" " +
                   "FROM \"Exercise\" WHERE \"exerciseName\" = :exerciseName";

            // 设置参数
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":exerciseName", OracleDbType.NVarchar2) { Value = exerciseName }
            };

            // 执行查询
            DataTable resultTable = OracleHelper.ExecuteTable(query, parameters);

            // 如果查询结果为空，返回null
            if (resultTable.Rows.Count == 0)
            {
                return null;
            }

            // 获取查询结果并赋值给Exercise类
            DataRow row = resultTable.Rows[0];
            Exercise exercise = new Exercise
            {
                exerciseName = row["exerciseName"].ToString(),
                explanation = row["explanation"].ToString(),
                equipmentName = row["equipmentName"].ToString(),
                part = row["part"].ToString(),
                gifUrl = row["gifUrl"].ToString(),
                time = Convert.ToInt32(row["time"]),
                count = Convert.ToInt32(row["count"]),
                coverUrl = row["coverUrl"].ToString()
            };

            return exercise;
        }
    }
}