namespace Glb.Common.Settings
{
    public class SeqSettings
    {
        public required string Host { get; init; }
        public int Port { get; init; }

        public string ServerUrl
        {
            get { return $"http://{Host}:{Port}"; }
        }
    }
}