using Fitness.DAL.Core;
using Fitness.Models;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DAL
{
    public class DatabasePingService : IHostedService, IDisposable
    {
        private Timer _timer;
        //private readonly string _connectionString = "your_connection_string_here"; // 替换为你的数据库连接字符串

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(PingDatabase, null, TimeSpan.Zero, TimeSpan.FromMinutes(0.5)); // 每半分钟发送一次查询
            return Task.CompletedTask;
        }

        private void PingDatabase(object state)
        {
            try
            {
                string sql = "SELECT 1 FROM \"User\"";
                OracleParameter[] oracleParameters = new OracleParameter[] { };
                OracleHelper.ExecuteTable(sql, oracleParameters);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
