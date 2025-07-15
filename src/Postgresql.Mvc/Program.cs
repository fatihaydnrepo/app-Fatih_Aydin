using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace Postgresql.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog konfigürasyonu
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.GrafanaLoki("http://loki.observability.svc.cluster.local:3100")
                .CreateLogger();

            builder.Host.UseSerilog();

            // OpenTelemetry Tracing
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Postgresql.Mvc"))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(opt =>
                        {
                            opt.Endpoint = new Uri("http://otel-collector.observability.svc.cluster.local:4317");
                        });
                });

            var app = builder.Build();
            app.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
