using System.Net;
using Glb.Common.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glb.Common.ProblemDetails;

public static class Extenstion
{
    public static IServiceCollection AddGlbProblemDetails(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<ControllerBase>>();

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
                    }
                }
        );

        services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
            options.Filters.Add(new GlbExceptionFilter(logger));
        });

        return services;
    }
}