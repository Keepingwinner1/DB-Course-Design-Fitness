using Fitness.DAL.Core;
using Fitness.Models;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class FitnessAIGuideDAL
    {

        // 成功插入一条健身截图记录时，返回这条记录的recordID
        public int InsertFitnessSuggestion(ScreenshotInfo screenshotInfo)
        {
            int screenshotID = -1;

            try
            {
                string sql = "INSERT INTO \"FitnessSuggestion\" (\"userID\", \"createTime\", \"exerciseName\", \"screenshotUrl\")" +
                    " VALUES (:\"userID\", :\"createTime\", :\"exerciseName\", :\"screenshotUrl\")" +
                    " RETURNING \"screenshotID\" INTO :\"screenshotID\"";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = screenshotInfo.userID },
                    new OracleParameter("\"createTime\"", OracleDbType.TimeStamp) { Value = DateTime.Now },
                    new OracleParameter("\"exerciseName\"", OracleDbType.NVarchar2) { Value = screenshotInfo.exerciseName },
                    new OracleParameter("\"screenshotUrl\"", OracleDbType.Clob) { Value = screenshotInfo.screenshotUrl },
                    new OracleParameter("\"screenshotID\"", OracleDbType.Int32) { Direction = ParameterDirection.Output }
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                screenshotID = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)oracleParameters[4].Value).Value);

                return screenshotID;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting meal record: {ex.Message}");
                throw;
            }
        }

        // 待大模型生成健身建议后更新数据库
        public bool UpdateSuggestion(int screenshotID, string suggestion)
        {

            try
            {
                string sql = "UPDATE \"FitnessSuggestion\" SET \"suggestions\"=:suggestion " +
                                "WHERE \"screenshotID\"=:screenshotID";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("suggestions", OracleDbType.Clob) { Value = suggestion},
                    new OracleParameter("screenshotID", OracleDbType.Int32) { Value = screenshotID },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 获取用户的所有的健身截图记录
        public DataTable GetFitnessSuggestionByUsrID(int userID)
        {
            try
            {
                string sql = "SELECT * FROM \"FitnessSuggestion\" WHERE \"userID\"=:\"userID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"userID\"", OracleDbType.Int32) { Value = userID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        // 删除健身截图记录
        public bool DeleteFitnessSuggestion(int screenshotID)
        {
            try
            {
                string sql = "DELETE FROM \"FitnessSuggestion\" WHERE \"screenshotID\" =:\"screenshotID\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("screenshotID", OracleDbType.Int32) { Value = screenshotID }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 获取健身截图记录的AI建议
        public DataTable GetSuggestionByID(int screenshotID)
        {
            try
            {
                string sql = "SELECT \"suggestions\" FROM \"FitnessSuggestion\" WHERE \"screenshotID\"=:\"screenshotID\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"screenshotID\"", OracleDbType.NVarchar2) { Value = screenshotID });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // 健身器材模块

        // 增
        public bool InsertFitnessEquipment(FitnessEquipment fitnessEquipment)
        {
            try
            {
                string sql = "INSERT INTO \"FitnessEquiOperation\"(\"equipmentName\", \"operationGuide\", \"briefIntr\", \"createTime\", \"lastUpdateTime\" , \"imgUrl\") " +
                             "VALUES(:\"equipmentName\", :\"operationGuide\", :\"briefIntr\", :\"createTime\", :\"lastUpdateTime\", :\"imgUrl\" )";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("\"equipmentName\"", OracleDbType.NVarchar2) { Value = fitnessEquipment.equipmentName },
                    new OracleParameter("\"operationGuide\"", OracleDbType.Clob) { Value = fitnessEquipment.operationGuide },
                    new OracleParameter("\"briefIntr\"", OracleDbType.Clob) { Value = fitnessEquipment.briefIntr },
                    new OracleParameter("\"createTime\"", OracleDbType.TimeStamp) { Value = DateTime.Now },
                    new OracleParameter("\"lastUpdateTime\"", OracleDbType.TimeStamp) { Value = DateTime.Now },
                    new OracleParameter("\"imgUrl\"", OracleDbType.Clob) { Value = fitnessEquipment.imgUrl },

                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while inserting food plan food: {ex.Message}");
                throw;
            }
        }

        // 删除某项健身器材
        public bool DeleteFitnessEquipmentByName(string equipmentName)
        {
            try
            {
                string sql = "DELETE FROM \"FitnessEquiOperation\" WHERE \"equipmentName\" =:\"equipmentName\"";
                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter("equipmentName", OracleDbType.NVarchar2) { Value = equipmentName }
                };
                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }

        }

        // 更新某项健身器材的信息
        public bool UpdateFitnessEquipment(FitnessEquipment fitnessEquipment)
        {

            try
            {
                string sql = "UPDATE \"FitnessEquiOperation\" SET \"imgUrl\"=:imgUrl, \"operationGuide\"=:operationGuide, \"briefIntr\"=:briefIntr, \"lastUpdateTime\"=:lastUpdateTime " +
                    
                                "WHERE \"equipmentName\"=:equipmentName";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {

                    new OracleParameter("imgUrl", OracleDbType.Clob) { Value = fitnessEquipment.imgUrl },
                    new OracleParameter("operationGuide", OracleDbType.Clob) { Value = fitnessEquipment.operationGuide },
                    new OracleParameter("briefIntr", OracleDbType.Clob) { Value = fitnessEquipment.briefIntr },
                    new OracleParameter("lastUpdateTime", OracleDbType.TimeStamp) { Value = DateTime.Now},
                    new OracleParameter("equipmentName", OracleDbType.NVarchar2) { Value = fitnessEquipment.equipmentName },
                };

                OracleHelper.ExecuteNonQuery(sql, null, oracleParameters);


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 获取健身器材的详情
        public DataTable GetEquipmentByName(string equipmentName)
        {
            try
            {
                string sql = "SELECT * FROM \"FitnessEquiOperation\" WHERE \"equipmentName\"=:\"equipmentName\"";
                DataTable dt = OracleHelper.ExecuteTable(sql,
                    new OracleParameter("\"equipmentName\"", OracleDbType.NVarchar2) { Value = equipmentName });

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // 获取所有健身器材的信息
        public DataTable GetALLEquipments()
        {
            try
            {
                string sql = "SELECT * FROM \"FitnessEquiOperation\" ";
                OracleParameter[] oracleParameters = new OracleParameter[] { };
                DataTable dt = OracleHelper.ExecuteTable(sql, oracleParameters);

                return dt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
