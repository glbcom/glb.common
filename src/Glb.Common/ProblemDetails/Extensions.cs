using Microsoft.Extensions.DependencyInjection;

namespace Glb.Common.ProblemDetails;

public static class Extenstion
{
    public static IServiceCollection AddGlbProblemDetails(this IServiceCollection services)
    {
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

        return services;
    }
}