using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Glb.Common.Base;
public abstract class GlbControllerBase : ControllerBase
{

    private Entities.GlbApplicationUser? _currentUser;

    [ApiExplorerSettings(IgnoreApi = true)]
    public bool IsInRole(string role)
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
                if (id != null)
                {
                    _currentUser = new Entities.GlbApplicationUser { Id = id };
                }

                if (_currentUser == null)
                {
                    return null;
                }

                var scopeClaims = User.FindAll("scope");
                //read the list of compnaies from the JSON file
                string? strCompanies = System.IO.File.ReadAllText("Assets/Companies.json");
                List<string>? lstCompanies = null;

                if (strCompanies != null)
                {
                    lstCompanies = JsonSerializer.Deserialize<List<string>>(strCompanies);
                }


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

}