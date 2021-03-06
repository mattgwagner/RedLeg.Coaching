using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using System;

namespace RedLeg.Coaching
{
    public class Program
    {
        public static LoggingLevelSwitch LogLevel { get; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((context, builder) =>
                {
                    Log.Logger =
                            new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                            .Enrich.WithProperty("Version", $"{typeof(Startup).Assembly.GetName().Version}")
                            .Enrich.WithProperty("MachineName", Environment.MachineName)
                            .WriteTo.Seq(serverUrl: context.Configuration.GetValue<string>("Seq:Endpoint"), apiKey: context.Configuration.GetValue<string>("Seq:ApiKey"), controlLevelSwitch: LogLevel)
                            .MinimumLevel.ControlledBy(LogLevel)
                            .CreateLogger();

                    builder.AddSerilog();
                });
    }
}
