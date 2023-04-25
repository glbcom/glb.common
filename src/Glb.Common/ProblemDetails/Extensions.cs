using System;
using System.Net;
using Glb.Common.Base;
using Glb.Common.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glb.Common.ProblemDetails;

public static class Extenstion
{
    public static IServiceCollection AddGlbProblemDetails(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<ControllerBase>>();
        var configuration = serviceProvider.GetService<IConfiguration>();


        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = (context) =>
                {
                    var gblProblemDetails = context.HttpContext.Features.Get<GlbProblemDetails>();
                    if (gblProblemDetails != null)
                    {

                        context.ProblemDetails.Detail = gblProblemDetails.Detail;
                        context.ProblemDetails.Extensions.Add("userId", gblProblemDetails.UserId);
                        context.ProblemDetails.Extensions.Add("compId", gblProblemDetails.CompId);
                        context.ProblemDetails.Instance = gblProblemDetails.Instance;
                        if (configuration != null)
                        {
                            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                            if (serviceSettings != null)
                            {
                                context.ProblemDetails.Extensions.Add("serviceName", serviceSettings.ServiceName);
                            }

                        }
                    }
                }
        );

        services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
            options.Filters.Add(new GlbExceptionFilter(logger, configuration));
        });

        return services;
    }
}