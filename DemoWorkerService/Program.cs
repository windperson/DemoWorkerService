using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;

namespace DemoWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.Configure<EventLogSettings>(config =>
                    {
                        config.LogName = $"{nameof(DemoWorkerService)}";
                        config.SourceName = $"{nameof(DemoWorkerService)} Source";
                    });
                }).UseWindowsService();
    }
}
