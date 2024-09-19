using Fitness.DAL.Core;
using Fitness.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class PaymentDAL
    {
        // 将DataRow转换为Payment对象
        public static Payment ToModel(DataRow row)
        {
            return new Payment
            {
                paymentID = Convert.ToInt32(row["paymentID"]),
                bookID = Convert.ToInt32(row["bookID"]),
                Amount = Convert.ToInt32(row["Amount"]),
                payMethod = Convert.ToString(row["payMethod"]),
                paymentStatus = Convert.ToInt32(row["paymentStatus"]),
                payTime = Convert.ToDateTime(row["payTime"])
            };
        }

        // 根据 bookID 获取 Payment 记录
        public static Payment GetByBookID(int bookID)
        {
            string sql = "SELECT \"paymentID\", \"bookID\", \"Amount\", \"payMethod\", \"paymentStatus\", \"payTime\" " +
                         "FROM \"Payment\" " +
                         "WHERE \"bookID\" = :bookID";

            OracleParameter[] parameters = {
            new OracleParameter("bookID", OracleDbType.Int32) { Value = bookID }
            };

            DataTable dt = OracleHelper.ExecuteTable(sql,parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            DataRow row = dt.Rows[0];
            return ToModel(row);
        }

        // 插入新的Payment记录
        public static int Insert(Payment payment,OracleTransaction transaction)
        {
            try
            {
                string sql = "INSERT INTO \"Payment\"(\"bookID\", \"Amount\", \"payMethod\", \"paymentStatus\", \"payTime\") " +
                             "VALUES(:bookID, :Amount, :payMethod, :paymentStatus, :payTime) " +
                             "RETURNING \"paymentID\" INTO :paymentID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("bookID", OracleDbType.Int32) { Value = payment.bookID },
                new OracleParameter("Amount", OracleDbType.Int32) { Value = payment.Amount },
                new OracleParameter("payMethod", OracleDbType.Char) { Value = payment.payMethod },
                new OracleParameter("paymentStatus", OracleDbType.Int32) { Value = payment.paymentStatus },
                new OracleParameter("payTime", OracleDbType.Date) { Value = payment.payTime },
                new OracleParameter("paymentID", OracleDbType.Int32, ParameterDirection.Output)
                };

                OracleHelper.ExecuteNonQuery(sql,transaction,parameters);
                OracleDecimal oracleInt = (OracleDecimal)parameters[5].Value;
                return oracleInt.ToInt32();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"插入失败: {ex.Message}");
                return -1;
            }
        }

        // 获取所有Payment记录
        public static List<Payment> GetAll()
        {
            List<Payment> payments = new List<Payment>();
            try
            {
                string sql = "SELECT * FROM \"Payment\"";
                DataTable dt = OracleHelper.ExecuteTable(sql);

                foreach (DataRow row in dt.Rows)
                {
                    payments.Add(ToModel(row));
                }

                return payments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有记录失败: {ex.Message}");
                return null;
            }
        }

        // 更新Payment记录的支付状态
        public static bool UpdatePaymentStatus(int paymentID, int paymentStatus)
        {
            //OracleConnection conn = null;
            OracleTransaction transaction = null;

            try
            {
/*                conn = OracleHelper.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();*/

                string sql = "UPDATE \"Payment\" SET \"paymentStatus\" = :paymentStatus WHERE \"paymentID\" = :paymentID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("paymentStatus", OracleDbType.Int32) { Value = paymentStatus },
                new OracleParameter("paymentID", OracleDbType.Int32) { Value = paymentID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);
/*                transaction.Commit();*/
                return true;
            }
            catch (Exception ex)
            {
/*                if (transaction != null)
                {
                    transaction.Rollback();
                }*/
                Console.WriteLine($"更新失败: {ex.Message}");
                return false;
            }
/*            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }*/
        }

    }
}
