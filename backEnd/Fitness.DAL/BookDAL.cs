using Fitness.DAL.Core;
using Fitness.Models;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Fitness.DAL
{
    public class BookDAL
    {
        //ToModel Insert GetAll UpdateBookStatusAndpaymentID

        // 将DataRow转换为Book对象
        public static Book ToModel(DataRow row)
        {
            return new Book
            {
                bookID = Convert.ToInt32(row["bookID"]),
                classID = Convert.ToInt32(row["classID"]),
                traineeID = Convert.ToInt32(row["traineeID"]),
                paymentID = Convert.ToInt32(row["paymentID"]),
                payMethod = row["payMethod"].ToString(),
                bookStatus = Convert.ToInt32(row["bookStatus"]),
                bookTime = Convert.ToDateTime(row["bookTime"])
            };
        }

        // 根据 bookID 获取预订记录
        public static Book GetBookByID(int bookID)
        {
            string sql = "SELECT \"bookID\", \"classID\", \"traineeID\", \"paymentID\", \"payMethod\", \"bookStatus\", \"bookTime\" " +
                         "FROM \"Book\" " +
                         "WHERE \"bookID\" = :bookID";

            OracleParameter[] parameters = {
            new OracleParameter("bookID", OracleDbType.Int32) { Value = bookID }
        };

            DataTable dt = OracleHelper.ExecuteTable(sql,parameters) ;
            if (dt.Rows.Count == 0)
                return null;
            
            DataRow row = dt.Rows[0];
            return ToModel(row);           
          }
    
        // 插入新的Book记录
        public static int Insert(Book book)
        {
            try
            {
                string sql = "INSERT INTO \"Book\"(\"classID\", \"traineeID\", \"paymentID\", \"payMethod\", \"bookStatus\", \"bookTime\") " +
                             "VALUES(:classID, :traineeID, :paymentID, :payMethod, :bookStatus, :bookTime) " +
                             "RETURNING \"bookID\" INTO :bookID";

                Console.WriteLine(book.classID);
                Console.WriteLine(book.payMethod);
                Console.WriteLine(book.traineeID);
                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("classID", OracleDbType.Int32) { Value = book.classID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = book.traineeID },
                new OracleParameter("paymentID", OracleDbType.Int32) { Value = -1 },
                new OracleParameter("payMethod", OracleDbType.Varchar2) { Value = book.payMethod },
                new OracleParameter("bookStatus", OracleDbType.Int32) { Value = 1},
                new OracleParameter("bookTime", OracleDbType.TimeStamp) { Value = DateTime.Now },
                new OracleParameter("bookID", OracleDbType.Int32, ParameterDirection.Output)
            };
                OracleHelper.ExecuteNonQuery(sql,null,parameters);
                OracleDecimal oracleInt = (OracleDecimal)parameters[6].Value;
                return oracleInt.ToInt32();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"插入失败: {ex.Message}");
                return -1;
            }
        }

        public static Book GetBookByClassIDAndUserID(int classID, int userID) {
            try
            {
                string sql = "SELECT * FROM \"Book\" WHERE \"classID\"= :classID AND \"traineeID\"= :userID";
                OracleParameter[] parameters = new OracleParameter[]
               {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = userID }
           };
                DataTable dt = OracleHelper.ExecuteTable(sql, parameters);
                return ToModel(dt.Rows[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有记录失败: {ex.Message}");
                return null;
            }
        }

        public static bool IsBooked(int classID, int userID) {
            try
            {
                string sql = "SELECT * FROM \"Book\" WHERE \"classID\"=:classID AND \"traineeID\"= :userID";
                OracleParameter[] parameters = new OracleParameter[]
               {
                new OracleParameter("classID", OracleDbType.Int32) { Value = classID },
                new OracleParameter("traineeID", OracleDbType.Int32) { Value = userID }
           };
                DataTable dt = OracleHelper.ExecuteTable(sql,parameters);
                int state = Convert.ToInt32(dt.Rows[0]["bookStatus"]);

                if (state == 2 || state == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有记录失败: {ex.Message}");
                return false;
            }
        }

        // 获取所有Book记录
        public static List<Book> GetAll()
        {
            List<Book> books = new List<Book>();
            try
            {
                string sql = "SELECT * FROM \"Book\"";
                DataTable dt = OracleHelper.ExecuteTable(sql);

                foreach (DataRow row in dt.Rows)
                {
                    books.Add(ToModel(row));
                }

                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有记录失败: {ex.Message}");
                return null;
            }
        }

        // 更新Book记录的状态和支付ID
        public static bool UpdateBookStatusAndPaymentID(int bookID, int bookStatus, int paymentID) {
            OracleTransaction transaction = null;

            try
            {
                string sql = "UPDATE \"Book\" SET \"bookStatus\" = :bookStatus ,\"paymentID\" =:paymentID WHERE \"bookID\" = :bookID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("bookStatus", OracleDbType.Int32) { Value = bookStatus },
                new OracleParameter("paymentID", OracleDbType.Int32) { Value = paymentID },
                new OracleParameter("bookID", OracleDbType.Int32) { Value = bookID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);

                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine($"更新失败: {ex.Message}");
                return false;
            }
        }
        public static bool UpdateBookStatus(int bookID, int bookStatus)
        {
            OracleTransaction transaction = null;

            try
            {
                string sql = "UPDATE \"Book\" SET \"bookStatus\" = :bookStatus WHERE \"bookID\" = :bookID";

                OracleParameter[] parameters = new OracleParameter[]
                {
                new OracleParameter("bookStatus", OracleDbType.Int32) { Value = bookStatus },
                new OracleParameter("bookID", OracleDbType.Int32) { Value = bookID }
                };

                OracleHelper.ExecuteNonQuery(sql, transaction, parameters);

                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine($"更新失败: {ex.Message}");
                return false;
            }
        }

    }
}




       
