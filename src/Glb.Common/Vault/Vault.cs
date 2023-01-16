using System.Collections.Generic;
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

        public static async Task<T?> ReadSecretAsync<T>(T ConfigObject, string MointPoint,
            string Path) where T : class
        {
            Secret<SecretData> secretValue = await vaultClient.V1.Secrets.KeyValue.V2
                               .ReadSecretAsync(path: Path, mountPoint: MointPoint);

            var configDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                JsonConvert.SerializeObject(ConfigObject)
            );
            Dictionary<string, object> secretDictionary = new Dictionary<string, object>(secretValue.Data.Data);

            //Get all the keys from vault and replace their values in the main configuration file object
            foreach (string key in secretDictionary.Keys)
            {
                string value = (string)secretDictionary[key];
                if (configDictionary != null && configDictionary.ContainsKey(key))
                {
                    configDictionary[key] = value;
                }
            }

            var result =
                JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(configDictionary));
            return result;
        }


        // public static async Task<Dictionary<string, object>> ReadSecretAsync<T>(T ConfigObject, string MointPoint,
        //     string Path) where T : class
        // {
        //     Secret<SecretData> secretValue = await vaultClient.V1.Secrets.KeyValue.V2
        //                        .ReadSecretAsync(path: Path, mountPoint: MointPoint);

        //     var configDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(
        //         JsonConvert.SerializeObject(ConfigObject)
        //     );
        //     Dictionary<string, object> secretDictionary = new Dictionary<string, object>(secretValue.Data.Data);

        //     //Get all the keys from vault and replace their values in the main configuration file object
        //     foreach (string key in secretDictionary.Keys)
        //     {
        //         string value = (string)secretDictionary[key];
        //         if (configDictionary != null && configDictionary.ContainsKey(key))
        //         {
        //             configDictionary[key] = value;
        //         }
        //     }

        //     return secretDictionary;
        // }
    }
}