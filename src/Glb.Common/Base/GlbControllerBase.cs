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
public abstract class GlbControllerBase<T> : ControllerBase where T : ControllerBase
{

    private Entities.GlbApplicationUser? _currentUser;
    private readonly ILogger<T> logger;

    protected GlbControllerBase(ILogger<T> logger)
    {
        this.logger = logger;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public bool CurrentUserIsInRole(string role)
    {

        return User.IsInRole(role);
    }
    public Entities.GlbApplicationUser? CurrentUser
    {
        get
        {

            if (User != null)
            {
                Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out Guid id);

                _currentUser = new Entities.GlbApplicationUser { Id = id };


                if (_currentUser == null)
                {
                    return null;
                }

                var scopeClaims = User.FindAll("scope");

                List<string> lstCompanies = GlbCompanies.Ids;


                if (lstCompanies != null)
                {
                    foreach (Claim clm in scopeClaims)
                    {
                        if (lstCompanies.Where(comp => comp.ToUpper() == clm.Value.ToUpper()).FirstOrDefault() != null)
                        {
                            _currentUser.ScopeCompId = clm.Value;
                            break;
                        }
                    }
                }
                else
                {
                    //to do log the error...
                    return _currentUser;
                }

                var compIdsClaims = User.FindAll("comp_ids");
                if (compIdsClaims != null)
                {
                    foreach (Claim compIdClaim in compIdsClaims)
                    {
                        _currentUser.CompIds.Add(compIdClaim.Value);
                    }
                }
                string? claimValue = null;
                _currentUser.UserName = User.FindAll("username").SingleOrDefault()?.Value;
                _currentUser.FirstName = User.FindAll("first_name").SingleOrDefault()?.Value;
                _currentUser.LastName = User.FindAll("last_name").SingleOrDefault()?.Value;
                _currentUser.MobileNumber = User.FindAll("mobile_number").SingleOrDefault()?.Value;
                _currentUser.Email = User.FindAll("email").SingleOrDefault()?.Value;
                claimValue = User.FindAll("mobile_number_confirmed").SingleOrDefault()?.Value;
                if (claimValue != null && claimValue.ToLower() == "true")
                {
                    _currentUser.MobileNumberConfirmed = true;
                }
                claimValue = User.FindAll("email_confirmed").SingleOrDefault()?.Value;
                if (claimValue != null && claimValue.ToLower() == "true")
                {
                    _currentUser.EmailConfirmed = true;
                }
                claimValue = User.FindAll("created_on").SingleOrDefault()?.Value;
                if (claimValue != null && DateTime.TryParse(claimValue, out DateTime createdOn) == true)
                {
                    _currentUser.CreatedOn = createdOn;

                }
                claimValue = User.FindAll("gender").SingleOrDefault()?.Value;
                if (claimValue != null && Enum.TryParse(typeof(Enums.Gender), claimValue, out object? gender) == true)
                {
                    _currentUser.Gender = (Enums.Gender)gender;

                }
            }

            return _currentUser;
        }

    }
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
    public override NotFoundResult NotFound()
    {
        if (CurrentUser != null)
        {
            logger.LogError("Not found userId:{1} compId:{2}", CurrentUser.Id, CurrentUser.ScopeCompId);
        }
        HttpContext.Features.Set(new GlbProblemDetails(null, Request.Path.Value, CurrentUser));
        return base.NotFound();
    }
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

    public ObjectResult StatusCode(string message, int statusCode, object? value)
    {
        return base.StatusCode(statusCode, new ResponseBase
        {
            Status = statusCode,
            Message = message,
            Data = value
        });
    }
    public override ObjectResult StatusCode(int statusCode, object? value)
    {
        return base.StatusCode(statusCode, new ResponseBase
        {
            Status = statusCode,
            Data = value
        });
    }
    public new OkObjectResult Ok()
    {
        return base.Ok(new ResponseBase
        {
            Status = (int)HttpStatusCode.OK
        });
    }

    public OkObjectResult Ok(string message, Object? value)
    {

        return base.Ok(new ResponseBase
        {
            Status = (int)HttpStatusCode.OK,
            Message = message,
            Data = value
        });

    }
    public override OkObjectResult Ok(Object? value)
    {
        if (value is string)
        {
            return base.Ok(new ResponseBase
            {
                Status = (int)HttpStatusCode.OK,
                Message = (string)value
            });
        }
        return base.Ok(new ResponseBase
        {
            Status = (int)HttpStatusCode.OK,
            Data = value
        });
    }
    public override CreatedAtActionResult CreatedAtAction(string? actionName, Object? value)
    {
        return base.CreatedAtAction(actionName, new ResponseBase
        {
            Status = (int)HttpStatusCode.Created,
            Data = value
        });
    }
    public override CreatedAtActionResult CreatedAtAction(string? actionName, Object? routValues, Object? value)
    {
        return base.CreatedAtAction(actionName, routValues, new ResponseBase
        {
            Status = (int)HttpStatusCode.Created,
            Data = value
        });
    }
    public override CreatedAtActionResult CreatedAtAction(string? actionName, string? controllerName, Object? routValues, Object? value)
    {
        return base.CreatedAtAction(actionName, controllerName, routValues, new ResponseBase
        {
            Status = (int)HttpStatusCode.Created,
            Data = value
        });
    }

}