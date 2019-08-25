namespace Common.Tracing
{
    public interface ITracingInfo
    {
        int RpcDepth { get; set; }

        string RequestId { get; set; }
    }
}