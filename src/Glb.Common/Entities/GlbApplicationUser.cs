using System;
using System.Collections.Generic;

namespace Glb.Common.Entities;
public class GlbApplicationUser
{

    public required Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public bool MobileNumberConfirmed { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool EmailConfirmed { get; set; }
    public Enums.Gender Gender { get; set; }
    public string? ScopeCompId { get; set; }
    public List<string> CompIds { get; set; } = new();
}