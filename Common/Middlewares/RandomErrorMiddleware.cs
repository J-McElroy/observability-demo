using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Middlewares
{
    public sealed class RandomErrorMiddleware
    {
        private readonly RequestDelegate _next;
        public RandomErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task Invoke(HttpContext context)
        {
            var random = new Random().Next(100);

            if (random % 20 == 0)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Server error");
                return;
            }

            if (random % 10 == 0)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Incorrect request");
                return;
            }
            
            if (random % 5 == 0)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Not found");
                return;
            }

            await _next.Invoke(context);
        }
    }
}