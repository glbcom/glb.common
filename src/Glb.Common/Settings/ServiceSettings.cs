namespace Glb.Common.Settings
{
    public class ServiceSettings
    {
        public required string ServiceName { get; init; }
        public required string Authority { get; init; }
        public required bool RequireHttpsMetadata { get; init; } = true;
        public required string MessageBroker { get; init; }
        public required string KeyVaultName { get; init; }
    }
}