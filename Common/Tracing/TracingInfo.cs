using System;
using System.Threading;

namespace Common.Tracing
{
    public class TracingInfo : ITracingInfo
    {
        private const string IdSeparator = "^";

        private static readonly AsyncLocal<int> _rpcDepth = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> _requestId = new AsyncLocal<string>();

        public int RpcDepth
        {
            get => _rpcDepth.Value;
            set => _rpcDepth.Value = value;
        }

        public string RequestId
        {
            get => _requestId.Value;
            set => _requestId.Value = value;
        }

        public static string GenerateRequestId(int rpcDepth, string oldId)
        {
            var newId = $"{rpcDepth}-{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
            var separator = string.IsNullOrEmpty(oldId) ? string.Empty: IdSeparator;
            newId = $"{oldId}{separator}{newId}";
            return newId;
        }
    }
}