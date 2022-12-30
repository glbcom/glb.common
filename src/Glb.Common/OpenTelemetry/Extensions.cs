using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Glb.Common.MassTransit;
using Glb.Common.Settings;

namespace Glb.Common.OpenTelemetry
{
    public static class Extensions
    {
        public static IServiceCollection AddTracing(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddOpenTelemetryTracing(builder =>
            {
                var serviceSettings = config.GetSection(nameof(ServiceSettings))
                                                   .Get<ServiceSettings>();
                if (serviceSettings != null)
                {
                    builder.AddSource(serviceSettings.ServiceName)
                                           .AddSource("MassTransit")
                                           .SetResourceBuilder(
                                               ResourceBuilder.CreateDefault()
                                                    .AddService(serviceName: serviceSettings.ServiceName))
                                            .AddHttpClientInstrumentation()
                                            .AddAspNetCoreInstrumentation()
                                            .AddJaegerExporter(options =>
                                            {
                                                var jaegerSettings = config.GetSection(nameof(JaegerSettings))
                                                                                  .Get<JaegerSettings>();
                                                if (jaegerSettings != null)
                                                {
                                                    options.AgentHost = jaegerSettings.Host;
                                                    options.AgentPort = jaegerSettings.Port;
                                                }
                                                else
                                                {
                                                    throw new System.Exception("jaegerSettings is null");
                                                }

                                            });
                }
                else
                {
                    throw new System.Exception("ServiceSettings is null");
                }

            })
            .AddConsumeObserver<ConsumeObserver>();

            return services;
        }

        public static IServiceCollection AddMetrics(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddOpenTelemetryMetrics(builder =>
            {
                var settings = config.GetSection(nameof(ServiceSettings))
                                            .Get<ServiceSettings>();
                if (settings != null)
                {
                    builder.AddMeter(settings.ServiceName)
                                         .AddMeter("MassTransit")
                                         .AddHttpClientInstrumentation()
                                         .AddAspNetCoreInstrumentation()
                                         .AddPrometheusExporter();
                }

            });

            return services;
        }
    }
}