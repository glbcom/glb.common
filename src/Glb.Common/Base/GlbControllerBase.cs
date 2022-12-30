using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Glb.Common.Base;
public class GlbControllerBase : ControllerBase
{

    private Entities.GlbApplicationUser? _currentUser;
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

                var compIdsClaims = User.FindAll("compids");
                if (compIdsClaims != null)
                {
                    foreach (Claim compIdClaim in compIdsClaims)
                    {
                        _currentUser.CompIds.Add(compIdClaim.Value);
                    }
                }

                if (scopeClaims != null)
                {
                    Claim? c = null;
                    c = scopeClaims.Where(claim => claim.Type == "first_name").FirstOrDefault();
                    if (c != null)
                    {
                        _currentUser.FirstName = c.Value;
                    }

                    c = scopeClaims.Where(claim => claim.Type == "last_name").FirstOrDefault();
                    if (c != null)
                    {
                        _currentUser.FirstName = c.Value;
                    }
                    c = scopeClaims.Where(claim => claim.Type == "mobile_number").FirstOrDefault();
                    if (c != null)
                    {
                        _currentUser.MobileNumber = c.Value;
                    }
                    c = scopeClaims.Where(claim => claim.Type == "email").FirstOrDefault();
                    if (c != null)
                    {
                        _currentUser.Email = c.Value;
                    }
                    c = scopeClaims.Where(claim => claim.Type == "mobile_number_confirmed").FirstOrDefault();
                    if (c != null)
                    {
                        if (c.Value.ToLower() == "true")
                            _currentUser.MobileNumberConfirmed = true;
                    }
                    c = scopeClaims.Where(claim => claim.Type == "created_on").FirstOrDefault();
                    if (c != null)
                    {
                        if (DateTime.TryParse(c.Value, out DateTime createdOn) == true)
                        {
                            _currentUser.CreatedOn = createdOn;
                        }

                    }
                    c = scopeClaims.Where(claim => claim.Type == "gender").FirstOrDefault();
                    if (c != null)
                    {
                        if (Enum.TryParse(typeof(Enums.Gender), c.Value, out object? gender) == true)
                        {
                            _currentUser.Gender = (Enums.Gender)gender;
                        }

                    }
                }


            }

            return _currentUser;
        }

    }

}