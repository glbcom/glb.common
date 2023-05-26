using System.Text.Json.Serialization;

namespace Glb.Common.Base;
public class GlbResponseBase
{
    public int Status { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RedirectAction { get; set; }

    public GlbResponseBase()
    {

    }
    public GlbResponseBase(int status, object data)
    {
        this.Status = status;
        this.Data = data;
    }
}