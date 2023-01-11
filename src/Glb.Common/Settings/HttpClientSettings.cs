namespace Glb.Common.Settings;
public class HttpClientSettings
{
    public long MaxResponseContentBufferSize { get; set; }
    public required string ClientName { get; set; }
    public required string BaseAddress { get; init; }
    public int RetryCount { get; init; }
    public int SleepDurationBaseInSeconds { get; init; }

    public int DurationOfBreakInSeconds { get; init; }
    public int TimeoutInSeconds { get; init; }
    public int handledEventsAllowedBeforeBreaking { get; init; }
}