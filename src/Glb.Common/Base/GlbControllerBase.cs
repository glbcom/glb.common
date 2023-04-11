using System;
using System.Net;
using Glb.Common.ProblemDetails;
using Glb.Common.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Glb.Common.Base;
public abstract class GlbControllerBase<T> : GlbMainControllerBase where T : ControllerBase
{
    #region Private methods
    private readonly ILogger<T> logger;
    private readonly ServiceSettings? serviceSettings;
    #region Logging

    private void LogMessageWithValue(LogLevel logLevel, string message, params object?[] args)
    {
        string formattedMessage = $"{message} userId:{CurrentUser?.Id} compId:{CurrentUser?.ScopeCompId}";

        switch (logLevel)
        {
            case LogLevel.Critical:
                logger.LogCritical(formattedMessage, args);
                break;
            case LogLevel.Error:
                logger.LogError(formattedMessage, args);
                break;
            case LogLevel.Warning:
                logger.LogWarning(formattedMessage, args);
                break;
            case LogLevel.Debug:
                logger.LogDebug(formattedMessage, args);
                break;
            case LogLevel.Information:
                logger.LogInformation(formattedMessage, args);
                break;
            case LogLevel.Trace:
                logger.LogTrace(formattedMessage, args);
                break;
        }
        GlbProblemDetails glbProblemDetails = new GlbProblemDetails(args?.ToString(), Request.Path.Value, CurrentUser);
        if (serviceSettings != null)
        {
            glbProblemDetails.ServiceName = serviceSettings.ServiceName;
        }
        HttpContext.Features.Set(glbProblemDetails);
    }
    #endregion
    #endregion

    #region Constructors
    protected GlbControllerBase(ILogger<T> logger)
    {
        this.logger = logger;
        this.serviceSettings = null;

    }
    protected GlbControllerBase(ILogger<T> logger, IConfiguration configuration)
    {
        this.logger = logger;
        this.serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()!;
    }
    #endregion

    #region Logging methods
    [ApiExplorerSettings(IgnoreApi = true)]
    public void LogError(string message, params object?[] args)
    {
        LogMessageWithValue(LogLevel.Error, message, args);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public void LogWarning(string message, params object?[] args)
    {
        LogMessageWithValue(LogLevel.Warning, message, args);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public void LogInformation(string message, params object?[] args)
    {
        LogMessageWithValue(LogLevel.Information, message, args);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public void LogDebug(string message, params object?[] args)
    {
        LogMessageWithValue(LogLevel.Debug, message, args);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public void LogTrace(string message, params object?[] args)
    {
        LogMessageWithValue(LogLevel.Trace, message, args);
    }
    #endregion

    #region StatusCode
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
    #endregion

    #region Not found and Bad Requests Sections
    [ApiExplorerSettings(IgnoreApi = true)]
    public new NotFoundResult NotFound(object? value)
    {
        this.LogWarning("Not Found", value);
        return base.NotFound();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public override NotFoundResult NotFound()
    {
        this.LogWarning("Not Found", null);
        return base.NotFound();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override BadRequestResult BadRequest()
    {
        this.LogWarning("Bad Request", null);
        return base.BadRequest();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new BadRequestResult BadRequest(object? value)
    {
        this.LogWarning("Bad Request", value);
        return base.BadRequest();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ForbidResult Forbid()
    {
        this.LogWarning("Forbid", null);
        return base.Forbid();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public ForbidResult Forbid(object? value)
    {
        this.LogWarning("Forbid", value);
        return base.Forbid();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ConflictResult Conflict()
    {
        this.LogError("Conflict", null);
        return base.Conflict();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new ConflictResult Conflict(object? value)
    {
        this.LogError("Conflict", value);
        return base.Conflict();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override UnauthorizedResult Unauthorized()
    {
        this.LogError("Unauthorized", null);
        return base.Unauthorized();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new UnauthorizedResult Unauthorized(object? value)
    {
        this.LogError("Unauthorized", value);
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

        LogInformation("Ok", value);
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