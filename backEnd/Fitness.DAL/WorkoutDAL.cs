using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness.DAL.Core;
using Newtonsoft.Json;
using System.Data;
using Fitness.DAL;
using Fitness.Models;

namespace Fitness.DAL
{
    public sealed class WorkoutDAL
    {
        private static readonly WorkoutDAL instance = new WorkoutDAL();
        private WorkoutDAL()
        {
        }
        public static WorkoutDAL Instance
        {
            get
            {
                return instance;
            }
        }

        public static List<Exercise> GetExercises(string workoutName)
        {
            string selectCommand = "SELECT \"exerciseName\" FROM \"Workout\" WHERE \"workoutName\" = :workoutName";
            OracleParameter[] parameters = new OracleParameter[]
            {
                new OracleParameter(":workoutName", OracleDbType.NVarchar2) { Value = workoutName }
            };

            DataTable exerciseNamesTable = OracleHelper.ExecuteTable(selectCommand, parameters);
            List<Exercise> exercises = new List<Exercise>();
            foreach (DataRow row in exerciseNamesTable.Rows)
            {
                string exerciseName = row["exerciseName"].ToString();
                Exercise exercise = ExerciseDAL.Get(exerciseName);
                if (exercise != null)
                {
                    exercises.Add(exercise);
                }
            }
            return exercises;
        }

        public static DataTable Get(string workoutName)
        {
            string workoutQuery = "SELECT \"exerciseName\", \"coverUrl\" FROM \"Workout\" WHERE \"workoutName\" = :workoutName";
            OracleParameter[] workoutParams =
            {
                        new OracleParameter(":workoutName", OracleDbType.NVarchar2) { Value = workoutName }
                    };
            DataTable workoutTable = OracleHelper.ExecuteTable(workoutQuery, workoutParams);
            return workoutTable;
        }
    }
}