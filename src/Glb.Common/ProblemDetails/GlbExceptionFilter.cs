using Glb.Common.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Glb.Common.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Glb.Common.ProblemDetails;

public class GlbExceptionFilter : IAsyncExceptionFilter, IActionFilter
{
    private GlbMainControllerBase? _controller { get; set; }
    private ILogger<ControllerBase>? _logger;

    public GlbExceptionFilter(ILogger<ControllerBase>? logger)
    {
        _logger = logger;
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
            var error = new GlbProblemDetails($"{context.Exception.Message}", null, _controller.CurrentUser);
            context.HttpContext.Features.Set(error);

            GlbApplicationUser? user = _controller.CurrentUser;
            if (_logger != null)
            {
                if (user != null)
                {
                    _logger.LogError("Exception Occured: " + context.Exception.Message + " userId: {0} and compId: {1}", user.Id, user.ScopeCompId);
                }
                else
                {
                    _logger.LogError("Exception Occured: " + context.Exception.Message);
                }

            }
            context.ExceptionHandled = false;
        }

        return Task.CompletedTask;
    }
}