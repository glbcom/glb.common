namespace Glb.Common.Base;
public class GlbResponseBase
{
    public int Status { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }

    public GlbResponseBase()
    {

    }
    public GlbResponseBase(int status, object data)
    {
        this.Status = status;
        this.Data = data;
    }
}