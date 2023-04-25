using Glb.Common.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Glb.Common.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Glb.Common.Settings;

namespace Glb.Common.ProblemDetails;

public class GlbExceptionFilter : IAsyncExceptionFilter, IActionFilter
{
    private GlbMainControllerBase? _controller { get; set; }
    private ILogger<ControllerBase>? _logger;
    private IConfiguration? _config;

    public GlbExceptionFilter(ILogger<ControllerBase>? logger, IConfiguration? config)
    {
        _logger = logger;
        _config = config;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        this._controller = (GlbMainControllerBase)context.Controller;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (_controller != null)
        {
            var error = new GlbProblemDetails($"{context.Exception.Message}", context.HttpContext.Request.Path.Value, _controller.CurrentUser);
            context.HttpContext.Features.Set(error);

            GlbApplicationUser? user = _controller.CurrentUser;


            if (_logger != null)
            {
                var serviceSettings = _config?.GetSection("ServiceSettings").Get<ServiceSettings>();
                if (user != null)
                {
                    _logger.LogError("Exception Occured: " + context.Exception.Message + "serviceName:{serviceName} userId: {userId} compId: {compId}", serviceSettings?.ServiceName, user.Id, user.ScopeCompId);
                }
                else
                {
                    _logger.LogError("Exception Occured: " + context.Exception.Message + "serviceName:{serviceName}", serviceSettings?.ServiceName);
                }

            }
            context.ExceptionHandled = false;
        }

        return Task.CompletedTask;
    }
}