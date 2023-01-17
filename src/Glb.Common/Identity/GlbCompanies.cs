using System.Collections.Generic;

namespace Glb.Common.Identity;
public static class GlbCompanies
{
    private static List<string>? _ids;

    public static List<string> Ids
    {

        get
        {
            _ids = new List<string> { "IDM", "CYB", "GDS", "CBV" };
            return _ids;
        }
    }

}