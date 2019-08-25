using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Tracing;
using CorrelationId;
using Serilog;

namespace Common.Communication
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor _correlationContext;
        private readonly ITracingInfo _tracingInfo;
        private readonly string _serviceName;

        public LoggingHandler(ICorrelationContextAccessor correlationContext, ITracingInfo tracingInfo, ServiceInfo serviceInfo)
        {
            _correlationContext = correlationContext;
            _tracingInfo = tracingInfo;
            _serviceName = serviceInfo?.ServiceName;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var method = request.Method;
            var requestUri = request.RequestUri;

            AddTracingHeaders(request);
            var logger = GetLoggerWithTracingInfo();

            await LogRequest(request, logger);
            var watch = Stopwatch.StartNew();

            try
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                watch.Stop();
                await LogResponse(response, logger, watch);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                
                throw new CommunicationException();
            }
            catch (HttpRequestException ex)
            {
                watch.Stop();
                (await EnrichLoggerContext(logger, request.Content, LoggingConstants.Request))
                    .ForContext(LoggingConstants.ErrorMessage, ex.Message)
                    .Error("Request {Method} {Url} failed after {ElapsedMs}", method, requestUri, watch.ElapsedMilliseconds);

                throw new CommunicationException(ex.Message, ex);
            }
            catch (TaskCanceledException ex)
            {
                watch.Stop();
                (await EnrichLoggerContext(logger, request.Content, LoggingConstants.Request))
                    .ForContext(LoggingConstants.ErrorMessage, ex.Message)
                    .Error("Request {Method} {Url} timeout after {ElapsedMs}", method, requestUri, watch.ElapsedMilliseconds);

                throw new CommunicationException("Task was cancelled, probably because of timeout", ex);
            }
        }

        private void AddTracingHeaders(HttpRequestMessage request)
        {
            if (_correlationContext.CorrelationContext != null)
            {
                request.Headers.TryAddWithoutValidation(_correlationContext.CorrelationContext.Header, _correlationContext.CorrelationContext.CorrelationId);
            }

            if (!string.IsNullOrEmpty(_serviceName))
            {
                request.Headers.TryAddWithoutValidation(TracingHeaders.RequestFrom, _serviceName);
            }
            var rpcDepth = _tracingInfo.RpcDepth;
            request.Headers.TryAddWithoutValidation(TracingHeaders.RpcDepth, rpcDepth.ToString());

            var requestId = _tracingInfo.RequestId;
            request.Headers.TryAddWithoutValidation(TracingHeaders.RequestId, requestId);
        }

        private ILogger GetLoggerWithTracingInfo()
        {
           var logger = Log
                .ForContext(LoggingConstants.RequestFrom, _serviceName)
                .ForContext(LoggingConstants.RequestId, _tracingInfo.RequestId);

           var rpcDepth = _tracingInfo.RpcDepth;
           if (rpcDepth > 0)
           {
                logger = logger.ForContext(LoggingConstants.RpcDepth, rpcDepth);
           }

           return logger;
        }

        private async Task LogRequest(HttpRequestMessage request, ILogger logger)
        {
            logger = await EnrichLoggerContext(logger, request?.Content, LoggingConstants.Request);

            logger.Debug("Outbound HTTP request sent {Method} {Url}", request?.Method, request?.RequestUri);
        }

        private async Task LogResponse(HttpResponseMessage response, ILogger logger, Stopwatch watch)
        {
            if (response == null)
            {
                logger.Warning("Empty response");
                return;
            }
            
            logger = await EnrichLoggerContext(logger, response.Content, LoggingConstants.Response);
                
            if (response.IsSuccessStatusCode)
            {
                logger.Debug("Request succeeded after {ElapsedMs}ms", watch.ElapsedMilliseconds);
            }
            else if ((int)response.StatusCode >= 500)
            {
                logger.Error("Request failed with {StatusCode} after {ElapsedMs}ms", (int)response.StatusCode, watch.ElapsedMilliseconds);
            }
            else
            {
                logger.Warning("Request failed with {StatusCode} after {ElapsedMs}ms", (int)response.StatusCode, watch.ElapsedMilliseconds);
            }
        }

        private async Task<ILogger> EnrichLoggerContext(ILogger logger, HttpContent content, string propertyPrefix)
        {
            var body = content != null ? await content.ReadAsStringAsync() : string.Empty;
            logger = logger.ForContext($"{propertyPrefix}-Length", body.Length);
            
            if (body.Length > 0)
            {
                logger.ForContext($"{propertyPrefix}-Context", body);
            }

            return logger;
        }
    }
}