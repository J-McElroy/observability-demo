using System;
using System.Threading;

namespace Common.Tracing
{
    public class TracingInfo : ITracingInfo
    {
        private const string IdSeparator = "^";

        private static readonly AsyncLocal<int> _rpcLevel = new AsyncLocal<int>();
        private static readonly AsyncLocal<string> _requestId = new AsyncLocal<string>();

        public int RpcLevel
        {
            get => _rpcLevel.Value;
            set => _rpcLevel.Value = value;
        }

        public string RequestId
        {
            get => _requestId.Value;
            set => _requestId.Value = value;
        }

        public static string GenerateRequestId(int rpcLevel, string oldId)
        {
            var newId = $"{rpcLevel}-{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
            var separator = string.IsNullOrEmpty(oldId) ? string.Empty: IdSeparator;
            newId = $"{oldId}{separator}{newId}";
            return newId;
        }
    }
}