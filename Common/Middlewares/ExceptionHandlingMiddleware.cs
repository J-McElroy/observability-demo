using System;
using System.Threading.Tasks;
using App.Metrics.Health.Logging;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Common.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private static readonly ILogger Logger = Log.ForContext<ExceptionHandlingMiddleware>();
        
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Request processing failed");
                try
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Request processing failed");
                }
                catch
                {
                    //
                }
            }
        }
    }
}