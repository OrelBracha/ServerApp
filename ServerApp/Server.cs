using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerApp.Models;
using ServerApp.Controllers;
using ServerApp.Services;

namespace ServerApp
{
    public class Server
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly UserController _userController = new UserController();
        private readonly UserBatchService _userBatchService = new UserBatchService();

        public Server(string uriPrefix)
        {
            _listener.Prefixes.Add(uriPrefix);
        }

        public async Task Start()
        {
            _listener.Start();
            Console.WriteLine("Server started. Listening for requests...");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/User/add")
                {
                    await HandleAddUser(request, response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/User/add-multiple")
                {
                    await HandleAddUsers(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/User/"))
                {
                    await HandleGetUser(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/User/all")
                {
                    await HandleGetAllUsers(response);
                }
                else if (request.HttpMethod == "PUT" && request.Url.AbsolutePath == "/User")
                {
                    await HandleUpdateUser(request, response);
                }
                else if (request.HttpMethod == "DELETE" && request.Url.AbsolutePath.StartsWith("/User/"))
                {
                    await HandleDeleteUser(request, response);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
            }
        }

        private async Task HandleAddUser(HttpListenerRequest request, HttpListenerResponse response)
        {
            var body = await new System.IO.StreamReader(request.InputStream).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<User>(body);
            var result = _userController.AddUser(user);

            response.StatusCode = (int)HttpStatusCode.OK;
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }

        private async Task HandleAddUsers(HttpListenerRequest request, HttpListenerResponse response)
        {
            var body = await new System.IO.StreamReader(request.InputStream).ReadToEndAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(body);
            var result = _userController.AddUsers(users);

            response.StatusCode = (int)HttpStatusCode.OK;
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }

        private async Task HandleGetUser(HttpListenerRequest request, HttpListenerResponse response)
        {
            var idStr = request.Url.AbsolutePath.Substring("/User/".Length);
            if (int.TryParse(idStr, out int id))
            {
                var result = _userController.GetUser(id);

                response.StatusCode = (int)HttpStatusCode.OK;
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            response.Close();
        }

        private async Task HandleGetAllUsers(HttpListenerResponse response)
        {
            var result = _userController.GetAllUsers();

            response.StatusCode = (int)HttpStatusCode.OK;
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }

        private async Task HandleUpdateUser(HttpListenerRequest request, HttpListenerResponse response)
        {
            var body = await new System.IO.StreamReader(request.InputStream).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<User>(body);
            var result = _userController.UpdateUser(user);

            response.StatusCode = (int)HttpStatusCode.OK;
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }

        private async Task HandleDeleteUser(HttpListenerRequest request, HttpListenerResponse response)
        {
            var idStr = request.Url.AbsolutePath.Substring("/User/".Length);
            if (int.TryParse(idStr, out int id))
            {
                var result = _userController.DeleteUser(id);

                response.StatusCode = (int)HttpStatusCode.OK;
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            response.Close();
        }

        private async Task HandleTriggerBatch(HttpListenerResponse response)
        {
            await _userBatchService.ProcessBatch();

            response.StatusCode = (int)HttpStatusCode.OK;
            var buffer = Encoding.UTF8.GetBytes("Batch process triggered successfully.");
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }
    }
}
