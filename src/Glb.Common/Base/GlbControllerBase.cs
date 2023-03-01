using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Glb.Common.Identity;
using Glb.Common.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glb.Common.Base;
public abstract class GlbControllerBase<T> : GlbMainControllerBase where T : ControllerBase
{
    private readonly ILogger<T> logger;

    protected GlbControllerBase(ILogger<T> logger)
    {
        this.logger = logger;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public ObjectResult StatusCode(string message, int statusCode, object? value)
    {
        return base.StatusCode(statusCode, new GlbResponseBase
        {
            Status = statusCode,
            Message = message,
            Data = value
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ObjectResult StatusCode(int statusCode, object? value)
    {
        return base.StatusCode(statusCode, new GlbResponseBase
        {
            Status = statusCode,
            Data = value
        });
    }

    #region Bad Requests Sections
    [ApiExplorerSettings(IgnoreApi = true)]
    public new NotFoundResult NotFound(object? value)
    {
        if (CurrentUser == null)
        {
            logger.LogError("{0}", value);
        }
        else
        {
            logger.LogError("{0} userId:{1} compId:{2}", value, CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails($"{value}", Request.Path.Value, CurrentUser));
        return base.NotFound();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public override NotFoundResult NotFound()
    {
        if (CurrentUser != null)
        {
            logger.LogError("Not found userId:{1} compId:{2}", CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails(null, Request.Path.Value, CurrentUser));
        return base.NotFound();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override BadRequestResult BadRequest()
    {
        if (CurrentUser != null)
        {
            logger.LogError("Bad Request userId:{1} compId:{2}", CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails(null, Request.Path.Value, CurrentUser));
        return base.BadRequest();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new BadRequestResult BadRequest(object? value)
    {
        if (CurrentUser == null)
        {
            logger.LogError("{0}", value);
        }
        else
        {
            logger.LogError("{0} userId:{1} compId:{2}", value, CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails($"{value}", Request.Path.Value, CurrentUser));
        return base.BadRequest();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ForbidResult Forbid()
    {
        if (CurrentUser != null)
        {
            logger.LogError("Unauthorized Request userId:{1} compId:{2}", CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails(null, Request.Path.Value, CurrentUser));
        return base.Forbid();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public ForbidResult Forbid(object? value)
    {
        if (CurrentUser == null)
        {
            logger.LogError("{0}", value);
        }
        else
        {
            logger.LogError("{0} userId:{1} compId:{2}", value, CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails($"{value}", Request.Path.Value, CurrentUser));
        return base.Forbid();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ConflictResult Conflict()
    {
        if (CurrentUser != null)
        {
            logger.LogError("Conflict Request userId:{1} compId:{2}", CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails(null, Request.Path.Value, CurrentUser));
        return base.Conflict();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new ConflictResult Conflict(object? value)
    {
        if (CurrentUser == null)
        {
            logger.LogError("{0}", value);
        }
        else
        {
            logger.LogError("{0} userId:{1} compId:{2}", value, CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails($"{value}", Request.Path.Value, CurrentUser));
        return base.Conflict();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override UnauthorizedResult Unauthorized()
    {
        if (CurrentUser != null)
        {
            logger.LogError("Conflict Request userId:{1} compId:{2}", CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails(null, Request.Path.Value, CurrentUser));
        return base.Unauthorized();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new UnauthorizedResult Unauthorized(object? value)
    {
        if (CurrentUser == null)
        {
            logger.LogError("{0}", value);
        }
        else
        {
            logger.LogError("{0} userId:{1} compId:{2}", value, CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails($"{value}", Request.Path.Value, CurrentUser));
        return base.Unauthorized();
    }
    #endregion

    #region  Success Requests Sections
    [ApiExplorerSettings(IgnoreApi = true)]
    public new OkObjectResult Ok()
    {
        return base.Ok(new GlbResponseBase
        {
            Status = (int)HttpStatusCode.OK
        });
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public OkObjectResult Ok(string message, Object? value)
    {

        return base.Ok(new GlbResponseBase
        {
            Status = (int)HttpStatusCode.OK,
            Message = message,
            Data = value
        });

    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public override OkObjectResult Ok(Object? value)
    {
        if (value is string)
        {
            return base.Ok(new GlbResponseBase
            {
                Status = (int)HttpStatusCode.OK,
                Message = (string)value
            });
        }
        return base.Ok(new GlbResponseBase
        {
            Status = (int)HttpStatusCode.OK,
            Data = value
        });
    }
    [ApiExplorerSettings(IgnoreApi = true)]

    public override CreatedAtActionResult CreatedAtAction(string? actionName, Object? value)
    {
        return base.CreatedAtAction(actionName, new GlbResponseBase
        {
            Status = (int)HttpStatusCode.Created,
            Data = value
        });
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public override CreatedAtActionResult CreatedAtAction(string? actionName, Object? routValues, Object? value)
    {
        return base.CreatedAtAction(actionName, routValues, new GlbResponseBase
        {
            Status = (int)HttpStatusCode.Created,
            Data = value
        });
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public override CreatedAtActionResult CreatedAtAction(string? actionName, string? controllerName, Object? routValues, Object? value)
    {
        return base.CreatedAtAction(actionName, controllerName, routValues, new GlbResponseBase
        {
            Status = (int)HttpStatusCode.Created,
            Data = value
        });
    }
    #endregion
}