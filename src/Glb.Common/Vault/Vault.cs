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

        public static async Task<T> ReadSecretAsync<T>(string mountPoint,
            string path, T? configurationObject = null) where T : class
        {
            Secret<SecretData> secretValue = await vaultClient.V1.Secrets.KeyValue.V2
                               .ReadSecretAsync(path: path, mountPoint: mountPoint);

            Dictionary<string, object>? configDictionary = new Dictionary<string, object>();

            if (configurationObject != null)
            {
                configDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                   JsonConvert.SerializeObject(configurationObject)
               );
                if (configDictionary == null)
                {
                    configDictionary = new Dictionary<string, object>();
                }
            }


            Dictionary<string, object> secretDictionary = new Dictionary<string, object>(secretValue.Data.Data);

            //Get all the keys from vault and replace their values in the main configuration file object
            foreach (string key in secretDictionary.Keys)
            {
                var value = secretDictionary[key];
                if (configDictionary != null)
                {
                    if (configDictionary.ContainsKey(key))
                    {
                        configDictionary[key] = value;
                    }
                    else
                    {
                        configDictionary.Add(key, value);
                    }
                }



            }
            var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(configDictionary));

            return result!;
        }
    }
}