using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerApp.Models;

namespace TestClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5212/") };

        static async Task Main(string[] args)
        {
            //await AddUsers();
            //await AddSingleUser();
            //await GetUser(4);
            await UpdateUser(3);
            await DeleteUser(11);
            await GetAllUsers();
        }

        static async Task AddUsers()
        {
            var users = new List<User> {
                new User { Name = "Eran Zahavi", Email = "eran.zahavi@example.com", Password = "pa$$sw0rd" },
                new User { Name = "Yosi Benayoun", Email = "yosi.benayoun@example.com", Password = "34343434" },
                new User { Name = "Lionel Messi", Email = "lionel.messi@example.com", Password = "56565656" },
                new User { Name = "Kylian M'bappe", Email = "mbappe@example.com", Password = "89898989"}
            };
            var content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("User/add-multiple", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task AddSingleUser()
        {
            var user = new User { Name = "David Green", Email = "david.green@example.com", Password = "abcd1234" };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("User/add", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task GetUser(int id)
        {
            var response = await client.GetAsync($"User/{id}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task GetAllUsers()
        {
            var response = await client.GetAsync("User/all");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task UpdateUser(int id)
        {
            var user = new User { ID = id, Name = "David Beckham", Email = "davidbeckham@example.com", Password = "football" };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("User", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        static async Task DeleteUser(int id)
        {
            var response = await client.DeleteAsync($"User/{id}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
