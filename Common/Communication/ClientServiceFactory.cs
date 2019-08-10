using System;
using System.Net.Http;
using Refit;

namespace Common.Communication
{
    public class ClientServiceFactory : IClientServiceFactory
    {
        public T Create<T>(ClientOptions options, HttpClient httpClient)
        {
            httpClient = ConfigureHttpClient<T>(options, httpClient);

            return RestService.For<T>(httpClient);
        }

        private static Uri GetBaseUrl<T>(ClientOptions options)
        {
            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
            {
                throw new InvalidOperationException($"BaseUrl {options.BaseUrl} is not valid for {typeof(T).Name}");
            }

            return baseUri;
        }

        private HttpClient ConfigureHttpClient<TClient>(ClientOptions options, HttpClient httpClient)
        {
            httpClient.BaseAddress = GetBaseUrl<TClient>(options);

            if (options.TimeoutSeconds > 0)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            }

            return httpClient;
        }
    }
}