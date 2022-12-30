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

    private Entities.GlbApplicationUser _currentUser;
    public Entities.GlbApplicationUser CurrentUser
    {
        get
        {
            if (User != null)
            {
                var scopeClaims = User.FindAll("scope");
                var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                //read the list of compnaies from the JSON file
                string strCompanies = System.IO.File.ReadAllText("Assets/Companies.json");
                List<string> lstCompanies = JsonSerializer.Deserialize<List<string>>(strCompanies);
                List<string> userCompanies = new List<string>();
                if (lstCompanies != null)
                {
                    foreach (Claim clm in scopeClaims)
                    {
                        _currentUser.ScopeCompId = lstCompanies.Where(comp => comp.ToUpper() == clm.Value.ToUpper()).FirstOrDefault();
                        if (!string.IsNullOrEmpty(_currentUser.ScopeCompId))
                        {
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
                foreach (Claim compIdClaim in compIdsClaims)
                {
                    _currentUser.CompIds.Add(compIdClaim.Value);
                }


                string[] claimTypes = { "username", "first_name", "last_name", "mobile_number", "email", "gender" };

                Claim c;
                c = scopeClaims.Where(claim => claim.Value == "first_name").FirstOrDefault();
                if (c != null)
                {
                    _currentUser.FirstName = c.Value;
                }

                c = scopeClaims.Where(claim => claim.Value == "last_name").FirstOrDefault();
                if (c != null)
                {
                    _currentUser.FirstName = c.Value;
                }
                c = scopeClaims.Where(claim => claim.Value == "mobile_number").FirstOrDefault();
                if (c != null)
                {
                    _currentUser.MobileNumber = c.Value;
                }
                c = scopeClaims.Where(claim => claim.Value == "mobile_number_confirmed").FirstOrDefault();
                if (c != null)
                {
                    if (c.Value.ToLower() == "true")
                        _currentUser.MobileNumberConfirmed = true;
                }
                c = scopeClaims.Where(claim => claim.Value == "created_on").FirstOrDefault();
                if (c != null)
                {
                    if (DateTime.TryParse(c.Value, out DateTime createdOn) == true)
                    {
                        _currentUser.CreatedOn = createdOn;
                    }

                }
                c = scopeClaims.Where(claim => claim.Value == "gender").FirstOrDefault();
                if (c != null)
                {
                    if (Enum.TryParse(typeof(Enums.Gender), c.Value, out object gender) == true)
                    {
                        _currentUser.Gender = (Enums.Gender)gender;
                    }

                }

            }

            return _currentUser;
        }

    }

}