using System;
using System.Collections.Generic;

namespace Glb.Common.Entities;
public class GlbApplicationUser
{

    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string MobileNumber { get; set; }
    public Enums.Gneder Gneder { get; set; }
    public string Email { get; set; }
    public required string ScopeCompId { get; set; }
    public List<string> CompIds { get; set; } = new();
    public HashSet<Guid> MessageIds { get; set; } = new();
}