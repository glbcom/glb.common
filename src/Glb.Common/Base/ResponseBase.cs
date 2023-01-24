namespace Glb.Common.Base;
public class ResponseBase
{
    public int Status { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }

    public ResponseBase()
    {

    }
    public ResponseBase(int status, object data)
    {
        this.Status = status;
        this.Data = data;
    }
}