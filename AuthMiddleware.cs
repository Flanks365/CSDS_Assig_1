using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CSDS_Assign_1
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly string[] pathsToIgnore =
        {
            "/login",
            "/loginpost",
            "/signup",
            "/logout",
            "/updateQuestions"
        };

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip checking for specific paths
            var path = context.Request.Path.ToString().ToLower();
            foreach (var pathIgnore in pathsToIgnore)
            {
                if (path == pathIgnore.ToLower())
                {
                    await _next(context);
                    return;
                }
            }

            //Checks if session is valid by checking whether USERNAME key exists in server session storage
            var username = context.Session.GetString("USERNAME");
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("username nullOrEmpty");
                context.Response.Redirect("login");
                return; // End the request
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

}
