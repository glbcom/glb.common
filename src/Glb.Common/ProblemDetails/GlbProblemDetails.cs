using System;
using Glb.Common.Entities;
namespace Glb.Common.ProblemDetails;

public class GlbProblemDetails
{
    public Guid UserId { get; set; }
    public string? CompId { get; set; }
    public string? Detail { get; set; }
    public string? ServiceName { get; set; }
    public string LogLevel { get; set; }
    public string? Instance { get; set; }
    public GlbProblemDetails(string? detail = null, string? instance = null, GlbApplicationUser? user = null)
    {
        if (user != null)
        {
            UserId = user.Id;
            CompId = user.ScopeCompId;
        }
        Instance = instance;
        Detail = detail;
        LogLevel = "error";
    }
}