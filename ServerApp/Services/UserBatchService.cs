using System;
using System.Linq;
using System.Timers;
using MySql.Data.MySqlClient;
using Dapper;
using ServerApp.Models;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace ServerApp.Services
{
    public class UserBatchService
    {
        private readonly string connectionString = "Server=localhost;Database=mydatabase;User=root;Password=orel12345678;";
        private Timer _timer;

        public UserBatchService()
        {
            _timer = new Timer(GetNextInterval());
            _timer.Elapsed += async (sender, e) => await ProcessBatch();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private double GetNextInterval()
        {
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 20, 0, 0, DateTimeKind.Local).AddDays(7);
            return (nextRun - now).TotalMilliseconds;
        }

        public async Task ProcessBatch()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var users = await connection.QueryAsync<User>("SELECT * FROM Users");
                foreach (var user in users)
                {
                    // Perform some batch processing (e.g., send an email)
                    Console.WriteLine($"Sending email to {user.Email}");

                }
                _timer.Interval = GetNextInterval();
            }
        }
    }
}
