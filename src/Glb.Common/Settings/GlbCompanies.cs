using System.Collections.Generic;

namespace Glb.Common.Settings;
public static class GlbCompanies
{
    private static List<string>? ids;

    public static List<string> Ids
    {
        get { return new() { "IDM", "CYB", "GDS", "CBV" }; }
    }


}