using App.Metrics.Health;
using Autofac;
using Common.Communication;
using Common.Extensions;
using Common.Tracing;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public abstract class BaseStartup
    {
        protected BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public abstract string ServiceName { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddCorrelationId();
            services.AddMvcCore()
                .AddJsonFormatters();
            
            var healthMetrics = AppMetricsHealth.CreateDefaultBuilder()
                .BuildAndAddTo(services);
            services.AddHealth(healthMetrics);
            services.AddHttpClient();
            
            services.AddTransient<LoggingHandler>();
            
            ConfigureServiceCollection(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCorrelationId(new CorrelationIdOptions { UseGuidForCorrelationId = true });
            app.UseHealthCheckEndpoint();
            app.UseRequestLogging();
            app.UseExceptionHandling();
            app.UseLatencyEmulation();
            app.UseMvc();
        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(new ServiceInfo() { ServiceName = ServiceName }).AsSelf();
            
            builder.RegisterType<ClientServiceFactory>()
                .As<IClientServiceFactory>()
                .SingleInstance();
            
            
            builder.RegisterType<TracingInfo>()
                .As<ITracingInfo>()
                .SingleInstance();

            builder.RegisterType<CorrelationContextAccessor>()
                .As<ICorrelationContextAccessor>()
                .SingleInstance();
            
            ConfigureDependencyInjections(builder);
        }

        protected virtual void ConfigureDependencyInjections(ContainerBuilder builder)
        {
        }
        
        protected virtual void ConfigureServiceCollection(IServiceCollection services)
        {
        }
        
    }
}