using System;
using System.Diagnostics;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Common
{
    public class WebHostRunner<TStartup> : IDisposable
        where TStartup : class
    {
        private readonly string[] _args;
        private readonly string _environmentName;
        private readonly string _serviceName;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private IConfiguration _configuration;
        private ILogger _logger;
        private IWebHost _webHost;

        public WebHostRunner(string serviceName, string[] args)
        {
            _serviceName = serviceName;
            _args = args;
            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            Console.Title = _serviceName;
            Console.WriteLine($"Initializing {_serviceName} for environment: {_environmentName}");
            Initialize();
            BuildWebHost();
        }
        
        public void Run()
        {
            StartWebHost();

            try
            {
                _stopwatch.Restart();
                _webHost.WaitForShutdown();
                _stopwatch.Stop();
                _logger.Warning("{ApplicationName} stopped after {ElapsedMs}ms", _serviceName, _stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }

                _logger.Fatal(ex, "{ApplicationName} stopped unexpectedly after {ElapsedMs}ms", _serviceName, _stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public void Dispose()
        {
            _webHost?.Dispose();
        }

        private void BuildWebHost()
        {
            try
            {
                _logger.Debug("{ServiceName} creating host", _serviceName);
                _stopwatch.Restart();
                
                _webHost = WebHost.CreateDefaultBuilder(_args)
                    .UseKestrel(x => x.AddServerHeader = false)
                    .UseSerilog(Log.Logger)
                    .UseConfiguration(_configuration)
                    .UseStartup<TStartup>()
                    .ConfigureServices((context, services) => { services.AddAutofac(); })
                    .Build();

                _stopwatch.Stop();

                _logger.Debug("{ServiceName} host created after {ElapsedMs}ms", _serviceName, _stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }

                _logger.Fatal(ex, "{ApplicationName} host creation failed after {ElapsedMs}ms", _serviceName, _stopwatch.ElapsedMilliseconds);
                Log.CloseAndFlush();
                throw;
            }
        }

        private void StartWebHost()
        {
            try
            {
                Log.Debug("{ApplicationName} starting host...", _serviceName);

                _stopwatch.Restart();
                _webHost.Start();

                _logger
                    .Information(
                    "{ApplicationName} host started with {Environment} configuration after {ElapsedMs}ms",
                    _serviceName,
                    _environmentName,
                    _stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }

                _logger.Fatal(ex, "{ApplicationName} host start failed after {ElapsedMs}ms", _serviceName, _stopwatch.ElapsedMilliseconds);
                Log.CloseAndFlush();
                throw;
            }
        }

        private void Initialize()
        {
            _stopwatch.Start();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddEnvironmentVariables()
                .AddCommandLine(_args)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", _environmentName)
                .Enrich.WithProperty("ApplicationName", _serviceName)
                .MinimumLevel.Debug()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            _logger = Log.ForContext<WebHostRunner<TStartup>>();

            _stopwatch.Stop();

            _logger.Debug("{ServiceName} initialized after {ElapsedMs}ms", _serviceName, _stopwatch.ElapsedMilliseconds);
        }
    }
}