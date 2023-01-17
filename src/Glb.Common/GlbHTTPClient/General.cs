using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Glb.Common.GlbHttpClient;
public static class General
{
    public static ByteArrayContent PrepareRequestBody(object RequestBody)
    {
        var signatureContent = JsonConvert.SerializeObject(RequestBody);
        var buffer = System.Text.Encoding.UTF8.GetBytes(signatureContent);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return byteContent;
    }
}