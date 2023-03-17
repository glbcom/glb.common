using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Glb.Common.GlbHttpClient;
public static class Helper
{
    public static ByteArrayContent PrepareRequestBody(object RequestBody, string? mediaType = null)
    {
        JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };
        var signatureContent = JsonConvert.SerializeObject(RequestBody, microsoftDateFormatSettings);
        var buffer = System.Text.Encoding.UTF8.GetBytes(signatureContent);
        var byteContent = new ByteArrayContent(buffer);
        if (mediaType == null)
        {
            mediaType = "application/json";
        }
        byteContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return byteContent;
    }
    public static async Task<T?> PostAsync<T>(HttpClient httpClient, string requestUri, object request, string? mediaType = null) where T : class
    {
        ByteArrayContent bodyContent = PrepareRequestBody(request, mediaType);
        var result = await httpClient.PostAsync(requestUri, bodyContent);
        string response = await result.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(response);
    }
}