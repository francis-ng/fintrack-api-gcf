using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using UserLogin.Models;
using MongoDB.Driver;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Text.Json;

namespace UserLogin
{
    public class Function : IHttpFunction
    {
        /// <summary>
        /// Logic for your function goes here.
        /// </summary>
        /// <param name="context">The HTTP context, containing the request and the response.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleAsync(HttpContext context)
        {
            if (context.Request.Method == "POST")
            {
                var client = new MongoClient("connectionString");
                var users = client.GetDatabase("databaseName").GetCollection<User>("userCollectionName");

                using (MemoryStream ms = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    var content = await JsonSerializer.DeserializeAsync(ms);
                }

                users.Find(user => user.UserName == name).FirstOrDefault();
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                await context.Response.CompleteAsync();
                return;
            }
        }
    }
}
