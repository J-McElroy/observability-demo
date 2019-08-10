namespace Common.Tracing
{
    public interface ITracingInfo
    {
        int RpcLevel { get; set; }

        string RequestId { get; set; }
    }
}