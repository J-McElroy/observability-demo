using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Tracing;
using CorrelationId;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using Serilog.Core.Enrichers;

namespace Common.Middlewares
{
    public sealed class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITracingInfo _tracingInfo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICorrelationContextAccessor _correlationContext;

        public LoggingMiddleware(RequestDelegate next, ITracingInfo tracingInfo, IHttpContextAccessor httpContextAccessor, ICorrelationContextAccessor correlationContext)
        {
            _next = next;
            _tracingInfo = tracingInfo;
            _httpContextAccessor = httpContextAccessor;
            _correlationContext = correlationContext;
        }

        public async Task Invoke(HttpContext context)
        {
            var propertyEnrichers = new List<PropertyEnricher>
            {
                new PropertyEnricher(LoggingConstants.SystemCorrelationId, _correlationContext.CorrelationContext.CorrelationId)
            };

            var method = context.Request.Method;
            var path = context.Request.Path;

            if (new[] { "options", "trace" }.Contains(method.ToLower()))
            {
                await _next(context);
                return;
            }

            var logger = Log
                .ForContext("Path", path)
                .ForContext("Method", method);

            if (!context.Request.Headers.ContainsKey(_correlationContext.CorrelationContext.Header))
            {
                logger = logger.ForContext("Root", true);
            }
            else if (context.Request.Headers.ContainsKey(TracingHeaders.RequestFrom))
            {
                logger = logger.ForContext(LoggingConstants.RequestFrom, context.Request.Headers[TracingHeaders.RequestFrom].ToString());
            }

            _tracingInfo.RpcLevel =
                int.TryParse(context.Request.Headers[TracingHeaders.RpcLevel], out var rpcLevel) ? ++rpcLevel : 0;

            propertyEnrichers.Add(new PropertyEnricher(LoggingConstants.RpcLevel, _tracingInfo.RpcLevel));

            var requestId = context.Request.Headers[TracingHeaders.RequestId].ToString();
            var newId = TracingInfo.GenerateRequestId(rpcLevel, requestId);
            _tracingInfo.RequestId = newId;

            context.TraceIdentifier = newId;
            _httpContextAccessor.HttpContext = context;
            
            var requestIdHash = $"{newId.GetHashCode():X}";

            using (LogContext.Push(propertyEnrichers.ToArray()))
            {
                logger.Information("Request {RequestIdHash} received {Method} {Path}", requestIdHash, method, path);

                var bodyStream = context.Response.Body;

                var stopWatch = new Stopwatch();
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    stopWatch.Start();
                    await _next(context);
                    stopWatch.Stop();
                    
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseBodyString = await new StreamReader(responseBody).ReadToEndAsync();

                    logger = GetContextLogger(logger, responseBodyString);

                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(bodyStream);
                }

                logger
                    .ForContext("StatusCode", context.Response.StatusCode)
                    .Information("Request {RequestIdHash} finished after {ElapsedMs}ms", requestIdHash, stopWatch.ElapsedMilliseconds);
            }
        }

        private ILogger GetContextLogger(ILogger logger, string responseBody)
        {
            logger = logger.ForContext("Response-Length", responseBody?.Length ?? 0);
            if (responseBody?.Length > 0)
            {
                logger = logger.ForContext("Response-Context", responseBody);
            }

            return logger;
        }
    }
}