using System.Threading.Tasks;
using Newtonsoft.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace Glb.Common.Vault
{
    public static class Vault
    {
        private static string VaultURL = "http://localhost:8200/";
        private static IAuthMethodInfo authMethod = new TokenAuthMethodInfo("some-root-token");
        private static VaultClientSettings vaultClientSettings =
            new VaultClientSettings(VaultURL, authMethod);
        private static IVaultClient vaultClient = new VaultClient(vaultClientSettings);

        public static async Task<T?> ReadSecretAsync<T>(string MointPoint, string Path) where T : class
        {
            Secret<SecretData> secretValue = await vaultClient.V1.Secrets.KeyValue.V2
                               .ReadSecretAsync(path: Path, mountPoint: MointPoint);
            var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(secretValue.Data.Data));
            return result;
        }
    }
}