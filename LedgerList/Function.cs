using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace LedgerList
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
            if (context.Request.Method == "GET")
            {

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                await context.Response.CompleteAsync();
            }
        }
    }
}
