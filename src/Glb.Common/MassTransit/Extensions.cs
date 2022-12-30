using System;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Glb.Common.Settings;

namespace Glb.Common.MassTransit
{
    public static class Extensions
    {
        private const string RabbitMq = "RABBITMQ";
        private const string ServiceBus = "SERVICEBUS";

        public static IServiceCollection AddMassTransitWithMessageBroker(
           this IServiceCollection services,
IConfiguration config,
Action<IRetryConfigurator>? configureRetries = null)
        {
            var serviceSettings = config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            if (serviceSettings != null && configureRetries != null)
            {
                switch (serviceSettings.MessageBroker?.ToUpper())
                {
                    case ServiceBus:
                        services.AddMassTransitWithServiceBus(configureRetries);
                        break;
                    case RabbitMq:
                    default:
                        services.AddMassTransitWithRabbitMq(configureRetries);
                        break;
                }
            }


            return services;
        }

        public static IServiceCollection AddMassTransitWithRabbitMq(
                    this IServiceCollection services,
         Action<IRetryConfigurator>? configureRetries = null)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());
                if (configureRetries != null)
                {
                    configure.UsingRabbitMq(configureRetries);
                }
            });

            return services;
        }

        public static IServiceCollection AddMassTransitWithServiceBus(
                    this IServiceCollection services,
        Action<IRetryConfigurator>? configureRetries = null)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());
                if (configureRetries != null)
                {
                    configure.UsingAzureServiceBus(configureRetries);
                }

            });

            return services;
        }

        public static void UsingMessageBroker(
            this IBusRegistrationConfigurator configure,
            IConfiguration config,
            Action<IRetryConfigurator>? configureRetries = null)
        {
            var serviceSettings = config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            if (serviceSettings != null && configureRetries != null)
            {
                switch (serviceSettings.MessageBroker?.ToUpper())
                {
                    case ServiceBus:
                        configure.UsingAzureServiceBus(configureRetries);
                        break;
                    case RabbitMq:
                    default:
                        configure.UsingRabbitMq(configureRetries);
                        break;
                }
            }

        }

        public static void UsingRabbitMq(
            this IBusRegistrationConfigurator configure,
            Action<IRetryConfigurator>? configureRetries = null)
        {
            configure.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                if (configuration != null)
                {
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    if (rabbitMQSettings != null)
                    {
                        configurator.Host(rabbitMQSettings.Host);
                    }
                    if (serviceSettings != null)
                    {
                        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    }

                    if (configureRetries == null)
                    {
                        configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    }

                    configurator.UseMessageRetry(configureRetries);
                    if (serviceSettings != null)
                    {
                        configurator.UseInstrumentation(serviceName: serviceSettings.ServiceName);
                    }
                }



            });
        }

        public static void UsingAzureServiceBus(
            this IBusRegistrationConfigurator configure,
 Action<IRetryConfigurator>? configureRetries = null)
        {
            configure.UsingAzureServiceBus((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                if (configuration != null)
                {
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var serviceBusSettings = configuration.GetSection(nameof(ServiceBusSettings)).Get<ServiceBusSettings>();
                    if (serviceBusSettings != null)
                    {
                        configurator.Host(serviceBusSettings.ConnectionString);
                    }
                    if (serviceSettings != null)
                    {
                        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    }
                    if (configureRetries == null)
                    {
                        configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    }

                    configurator.UseMessageRetry(configureRetries);
                    if (serviceSettings != null)
                    {
                        configurator.UseInstrumentation(serviceName: serviceSettings.ServiceName);
                    }


                }


            });
        }
    }
}