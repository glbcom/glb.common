using Microsoft.AspNetCore.Mvc;

namespace Glb.Common.Base;
public class GlbControllerBase : ControllerBase
{
    public Entities.GlbApplicationUser CurrentUser { get; set; }

}