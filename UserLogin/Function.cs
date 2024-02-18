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
using Isopoh.Cryptography.Argon2;
using System.Text;
using System;

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

                AuthForm content = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    content = await JsonSerializer.DeserializeAsync<AuthForm>(ms);
                }

                var user = users.Find(user => user.UserName == content.UserName).FirstOrDefault();
                if (user != null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.CompleteAsync();
                    return;
                }

                var argon2Config = new Argon2Config
                {
                    Threads = Environment.ProcessorCount,
                    Password = Encoding.UTF8.GetBytes(content.Password),
                    Salt = user.Salt,
                    HashLength = 128
                };
                if (Argon2.Verify(content.Password, argon2Config))
                {
                    var accessToken = Auth.GenerateJWToken(_config, user.UserName);
                    var refreshToken = Auth.GenerateRefreshToken(_config, user.UserName);
                }
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
