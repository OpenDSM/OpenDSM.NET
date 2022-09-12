global using static OpenDSM.NET.Requests.Requests;

namespace OpenDSM.NET.Requests;
internal class Requests
{
    /// <summery>The host url</summery>
#if DEBUG
    private static readonly string HOST = "http://127.0.0.1:8080";
#else
    private static readonly string HOST = "http://api.opendsm.tk";
#endif

    // GET DATA
    public static HttpResponseMessage Get(DSMClient client, string path) => Make(HttpMethod.Get, client, path);
    public static HttpResponseMessage Get(DSMClient client, string path, IEnumerable<KeyValuePair<string, string>>? formData = null) => Make(HttpMethod.Get, client, path, formData: formData);

    // POST DATA
    public static HttpResponseMessage Post(DSMClient client, string path) => Make(HttpMethod.Post, client, path);
    public static HttpResponseMessage Post(DSMClient client, string path, string body) => Make(HttpMethod.Post, client, path, body);
    public static HttpResponseMessage Post(DSMClient client, string path, IEnumerable<KeyValuePair<string, string>>? formData) => Make(HttpMethod.Post, client, path, formData: formData);

    // DELETE DATA
    public static HttpResponseMessage Delete(DSMClient client, string path) => Make(HttpMethod.Delete, client, path);
    public static HttpResponseMessage Delete(DSMClient client, string path, string body) => Make(HttpMethod.Delete, client, path, body);
    public static HttpResponseMessage Delete(DSMClient client, string path, IEnumerable<KeyValuePair<string, string>>? formData = null) => Make(HttpMethod.Delete, client, path, formData: formData);

    // PATCH DATA
    public static HttpResponseMessage Patch(DSMClient client, string path) => Make(HttpMethod.Patch, client, path);
    public static HttpResponseMessage Patch(DSMClient client, string path, string body) => Make(HttpMethod.Patch, client, path, body);
    public static HttpResponseMessage Patch(DSMClient client, string path, IEnumerable<KeyValuePair<string, string>>? formData = null) => Make(HttpMethod.Patch, client, path, formData: formData);

    private static HttpResponseMessage Make(HttpMethod method, DSMClient client, string path, string? body = "", IEnumerable<KeyValuePair<string, string>>? formData = null)
    {
        HttpClient http = client.client;
        using HttpRequestMessage request = new(method, $"{HOST}{path}");
        if (!string.IsNullOrWhiteSpace(body))
            request.Content = new StringContent(body, System.Text.Encoding.UTF8);
        else if (formData != null)
            request.Content = new FormUrlEncodedContent(formData);
        HttpResponseMessage response = http.Send(request);
        return response;
    }
}