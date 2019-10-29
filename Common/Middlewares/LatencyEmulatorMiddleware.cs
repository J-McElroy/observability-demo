using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Middlewares
{
    public class LatencyEmulatorMiddleware
    {
        private static readonly Random Random = new Random();
        private readonly RequestDelegate _next;

        public LatencyEmulatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await Task.Delay(Random.Next(5) * 100);
            await _next(context);
        }
    }
}