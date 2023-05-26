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

        string problemDetailString = message;
        if (args != null && args.Length > 0)
        {
            // Get all text between curly braces
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(problemDetailString, @"\{.*?\}");

            if (matches.Count > 0 && matches.Count <= args.Length)
            {
                // Replace the text between curly braces with the values in the args array
                for (int i = 0; i < matches.Count; i++)
                {

                    problemDetailString = problemDetailString.Replace(matches[i].Value, args[i]?.ToString());
                }
            }
        }

        message = message + " serviceName:{serviceName} userId:{userId} compId:{compId}";

        // Add the original args to the newArgs array first
        int argsLength = args == null ? 0 : args.Length;
        object?[] newArgs = new object?[argsLength + 3];
        if (args != null)
        {
            for (int i = 0; i < argsLength; i++)
            {
                newArgs[i] = args[i];
            }
        }


        // Add the three new parameters at the end
        newArgs[argsLength] = serviceSettings == null ? "" : serviceSettings.ServiceName;
        newArgs[argsLength + 1] = CurrentUser?.Id;
        newArgs[argsLength + 2] = CurrentUser?.ScopeCompId;
        args = newArgs;


        switch (logLevel)
        {
            case LogLevel.Critical:
                logger.LogCritical(message, args);
                break;
            case LogLevel.Error:
                logger.LogError(message, args);
                break;
            case LogLevel.Warning:
                logger.LogWarning(message, args);
                break;
            case LogLevel.Debug:
                logger.LogDebug(message, args);
                break;
            case LogLevel.Information:
                logger.LogInformation(message, args);
                break;
            case LogLevel.Trace:
                logger.LogTrace(message, args);
                break;
        }

        GlbProblemDetails glbProblemDetails = new GlbProblemDetails(problemDetailString, Request.Path.Value, CurrentUser);
        if (serviceSettings != null)
        {
            glbProblemDetails.ServiceName = serviceSettings.ServiceName;
        }
        glbProblemDetails.LogLevel = logLevel.ToString();
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
        this.LogWarning("Not Found :{0}", value);
        return base.NotFound();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public override NotFoundResult NotFound()
    {
        this.LogWarning("Not Found");
        return base.NotFound();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override BadRequestResult BadRequest()
    {
        this.LogWarning("Bad Request");
        return base.BadRequest();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new BadRequestResult BadRequest(object? value)
    {
        this.LogWarning("Bad Request :{0}", value);
        return base.BadRequest();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ForbidResult Forbid()
    {
        this.LogWarning("Forbid");
        return base.Forbid();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public ForbidResult Forbid(object? value)
    {
        this.LogWarning("Forbid :{0}", value);
        return base.Forbid();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override ConflictResult Conflict()
    {
        return base.Conflict();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new ConflictResult Conflict(object? value)
    {
        this.LogError("Conflict :{0}", value);
        return base.Conflict();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public override UnauthorizedResult Unauthorized()
    {
        this.LogError("Unauthorized");
        return base.Unauthorized();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public new UnauthorizedResult Unauthorized(object? value)
    {
        this.LogError("Unauthorized :{0}", value);
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
        int statusCode = (int)HttpStatusCode.OK;
        if (value != null && value is string) statusCode = (int)HttpStatusCode.Redirect;

        if (statusCode == (int)HttpStatusCode.Redirect) LogInformation("Redirect :{0}", value);
        else LogInformation("Ok :{0}", value);

        if (statusCode == (int)HttpStatusCode.Redirect)
        {
            return base.Ok(new GlbResponseBase
            {
                Status = statusCode,
                Message = message,
                RedirectAction = value != null ? (string)value : null
            });
        }
        else
        {
            return base.Ok(new GlbResponseBase
            {
                Status = statusCode,
                Message = message,
                Data = value
            });
        }
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public OkObjectResult Ok(string message, Object? value, string redirectAction)
    {
        int statusCode = (int)HttpStatusCode.OK;
        if (!string.IsNullOrWhiteSpace(redirectAction)) statusCode = (int)HttpStatusCode.Redirect;

        if (statusCode == (int)HttpStatusCode.Redirect) LogInformation("Redirect :{0}", value);
        else LogInformation("Ok :{0}", value);

        return base.Ok(new GlbResponseBase
        {
            Status = statusCode,
            Message = message,
            Data = value,
            RedirectAction = redirectAction
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