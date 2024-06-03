using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Dapper;
using ServerApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly string connectionString = "Server=localhost;Database=mydatabase;User=root;Password=orel12345678;";

        [HttpPost("add")]
        public IActionResult AddUser(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password); SELECT LAST_INSERT_ID();";
                var userId = connection.Query<int>(sql, user).Single();
                user.ID = userId;
                return Ok(user);
            }
        }

        [HttpPost("add-multiple")]
        public IActionResult AddUsers(List<User> users)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
                var result = connection.Execute(sql, users);
                return Ok(users);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM Users WHERE ID = @ID";
                var user = connection.QuerySingleOrDefault<User>(sql, new { ID = id });
                return Ok(user);
            }
        }
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM Users";
                var users = connection.Query<User>(sql).ToList();
                return Ok(users);
            }
        }


        [HttpPut]
        public IActionResult UpdateUser(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE Users SET Name = @Name, Email = @Email, Password = @Password WHERE ID = @ID; SELECT * FROM Users WHERE ID = @ID";
                var updatedUser = connection.QuerySingleOrDefault<User>(sql, user);
                return Ok(updatedUser);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var getUserSql = "SELECT * FROM Users WHERE ID = @ID; DELETE FROM Users WHERE ID = @ID";
                var deletedUser = connection.QuerySingleOrDefault<User>(getUserSql, new { ID = id });
                return Ok(deletedUser);
            }
        }
    }
}
