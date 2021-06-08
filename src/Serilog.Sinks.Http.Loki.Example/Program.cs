using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Context;
using Serilog.Sinks.Http.Loki.Labels;

namespace Serilog.Sinks.Http.Loki.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var credentials = new NoAuthCredentials("http://192.168.2.202:3101");
            var provider = new DefaultLogLabelProvider();
            provider.AddPropertiesAsLabels("AppName", "SpecialCode");

            var log = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("AppName", "SerilogDebugger")
                        .Enrich.WithThreadId()
                        .WriteTo.Console()
                        .WriteTo.HttpLoki(credentials, logLabelProvider: provider)
                        .CreateLogger();
            Log.Logger = log;

            log.Verbose("Hello, Serilog.");

            log.Information("[{SpecialCode}]Information Text", "SerilogSinksHttpLoki");

            var total = 1000;
            for (var i = 1; i < total + 1; i++)
            {
                log.Debug("Processing item {ItemIndex} of {TotalItems}", i, total);
            }

            try
            {
                var invalidCast = (string)new object();
            }
            catch (Exception e)
            {
                log.Error(e, "Exception due to invalid cast");
            }

            var position = new { Latitude = 25, Longitude = 134 };
            log.Information("3# Random message processed {@Position} in {Elapsed:000} ms.", position, 34);

            using (LogContext.PushProperty("A", 1))
            {
                log.Warning("Warning with Property A");
                log.Fatal("Fatal with Property A");
            }

            using (LogContext.PushProperty("MyAppendPropertyName", 1))
            {
                log.Warning("Warning with Property MyAppendPropertyName");
                log.Fatal("Fatal with Property MyAppendPropertyName");
            }

            log.Verbose("Goodbye, Serilog.");

            log.Dispose();
            Log.CloseAndFlush();

            Console.ReadKey();
        }
    }
}
