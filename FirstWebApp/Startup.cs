﻿using System;
using Autofac;
using Common;
using Common.Communication;
using Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace FirstWebApp
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override string ServiceName => "FirstService";

        protected override void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddHttpClient("clientwithhandlers")
                .AddHttpMessageHandler<LoggingHandler>()
                .AddTransientHttpErrorPolicy(p => 
                    p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));;
        }

        protected override void ConfigureDependencyInjections(ContainerBuilder builder)
        {
            builder.RegisterApiClient<ISecondServiceClient>("clientwithhandlers");
        }
    }
}