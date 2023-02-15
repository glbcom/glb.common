using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Glb.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace Glb.Common.GlbHttpClient;
public static class Extensions
{
    public static IServiceCollection AddGlbHttpClient<T>(
            this IServiceCollection services, Action<HttpClient> configureClient, IConfiguration config, String clientName)
               where T : class
    {
        //System.Action<System.IServiceProvider, System.Net.Http.HttpClient> configureClient
        Random jitterer = new Random();
        var glbHttpClientSettings = config.GetSection(nameof(HttpClientSettings)).
                Get<List<HttpClientSettings>>()!.Where(n => n.ClientName ==
                clientName).FirstOrDefault();

        //configureClient = Action<IServiceProvider, HttpClient>;
        if (glbHttpClientSettings == null)
        {
            return services;
        }
        services.AddHttpClient<T>((c) =>
        {
            c.BaseAddress = new Uri(glbHttpClientSettings.BaseAddress);
        })

          .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
              glbHttpClientSettings.RetryCount,
              retryAttempt => TimeSpan.FromSeconds(Math.Pow(glbHttpClientSettings.SleepDurationBaseInSeconds, retryAttempt))
                              + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
              onRetry: (outcome, timespan, retryAttempt) =>
              {
                  var serviceProvider = services.BuildServiceProvider();
                  serviceProvider.GetService<ILogger<T>>()?
                      .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
              }
          ))
          .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
              glbHttpClientSettings.HandledEventsAllowedBeforeBreaking,
              TimeSpan.FromSeconds(glbHttpClientSettings.DurationOfBreakInSeconds),
              onBreak: (outcome, timespan) =>
              {
                  var serviceProvider = services.BuildServiceProvider();
                  serviceProvider.GetService<ILogger<T>>()?
                      .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
              },
              onReset: () =>
              {
                  var serviceProvider = services.BuildServiceProvider();
                  serviceProvider.GetService<ILogger<T>>()?
                      .LogWarning($"Closing the circuit...");
              }
          ))
          .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(glbHttpClientSettings.TimeoutInSeconds));
        return services;

    }
    public static IServiceCollection AddGlbHttpClient<T>(
           this IServiceCollection services, IConfiguration config, String clientName)
           where T : class
    {

        var glbHttpClientSettings = config.GetSection(nameof(HttpClientSettings)).Get<List<HttpClientSettings>>()!.Where(n => n.ClientName == clientName).FirstOrDefault();

        //configureClient = Action<IServiceProvider, HttpClient>;
        if (glbHttpClientSettings == null)
        {
            return services;
        }

        HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(glbHttpClientSettings.BaseAddress)
        };

        return services.AddGlbHttpClient<T>(configureClient => { configureClient = httpClient; }, config, clientName);

    }
   
}