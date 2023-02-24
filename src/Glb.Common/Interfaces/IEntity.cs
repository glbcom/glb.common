using System;
using System.Collections.Generic;

namespace Glb.Common.Inerfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }
        HashSet<Guid> MessageIds { get; set; }
    }
}