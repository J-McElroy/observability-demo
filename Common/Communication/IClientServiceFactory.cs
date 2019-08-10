using System.Net.Http;

namespace Common.Communication
{
    public interface IClientServiceFactory
    {
        T Create<T>(ClientOptions options, HttpClient httpClient);
    }
}