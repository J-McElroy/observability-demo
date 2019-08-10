using System.Net.Http;
using Autofac;
using Autofac.Builder;
using Common.Communication;
using Microsoft.Extensions.Configuration;

namespace Common.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterApiClient<T>(
            this ContainerBuilder builder,
            string httpClientName = null)
        {
            return builder.Register(c =>
            {
                var clientServiceFactory = c.Resolve<IClientServiceFactory>();
                var config = c.Resolve<IConfiguration>();
                var httpClientFactory = c.Resolve<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient(httpClientName);
                var options = GetOptionsForClient<T>(config);

                var instance = clientServiceFactory.Create<T>(options, client);
                return instance;
            }).As<T>();
        }
        
        private static ClientOptions GetOptionsForClient<TClient>(IConfiguration configuration)
        {
            var clientName = typeof(TClient).Name;
            var options = configuration.GetSection($"Services:{clientName}").Get<ClientOptions>();
            return options;
        }
    }
}