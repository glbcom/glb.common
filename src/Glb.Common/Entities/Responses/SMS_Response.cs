namespace Glb.Common.Entities.Responses;
public class SMS_Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Code { get; set; }
        public string? Exception { get; set; }
    }