using System;
using Autofac;
using Common;
using Common.Communication;
using Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Contracts;
using Polly;
using Serilog;

namespace GatewayService
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override string ServiceName => "GatewayService";

        protected override void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddHttpClient("clientwithhandlers")
                .AddHttpMessageHandler<LoggingHandler>()
                .AddTransientHttpErrorPolicy(p => 
                    p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600),
                        (response, calculatedWaitDuration) => { Log.Warning(
                            response.Exception, "Request failed with {ResponseStatusCode}", (int?)response.Result?.StatusCode ?? -1 ); }));
        }

        protected override void ConfigureDependencyInjections(ContainerBuilder builder)
        {
            builder.RegisterApiClient<IOrderServiceClient>("clientwithhandlers");
        }
    }
}