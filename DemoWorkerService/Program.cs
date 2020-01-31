using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;

namespace DemoWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddApplicationInsightsTelemetryWorkerService();
                }).UseSerilog((hostBuilderContext, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);

                    var telemetryClient = CreateTelemetryClient(hostBuilderContext);
                    if(telemetryClient != null)
                    {
                        loggerConfiguration.WriteTo.ApplicationInsights(telemetryClient, TelemetryConverter.Traces);
                    }
                });

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                hostBuilder
                    .UseWindowsService();
            }

            return hostBuilder;
        }

        private static TelemetryClient CreateTelemetryClient(HostBuilderContext hostBuilderContext)
        {
            var key = hostBuilderContext.Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
            if(string.IsNullOrEmpty(key))
            {
                return null;
            }

            var provider = new ServiceCollection()
                .AddApplicationInsightsTelemetryWorkerService(key).BuildServiceProvider();
            var telemetryClient = provider.GetRequiredService<TelemetryClient>();
            telemetryClient.InstrumentationKey = key;
            return telemetryClient;
        }
    }
}
