using Common.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthCheckEndpoint(this IApplicationBuilder builder)
        {
            return builder.Map("/health", app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("OK");
                });
            });
        }
        
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
        
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
        
        public static IApplicationBuilder UseLatencyEmulation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LatencyEmulatorMiddleware>();
        }
    }
}