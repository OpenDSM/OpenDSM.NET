using OpenDSM.NET.Requests;

namespace OpenDSM.NET;
/// <summary>
/// The client that will make all of the requests
/// </summary>
public sealed class DSMClient
{
    private readonly string api_key;
    internal readonly HttpClient client;
    public readonly UserRequests Users;
#if DEBUG
    private readonly string HOST = "http://127.0.0.1:8080";
#else
    private readonly string HOST = "http://api.opendsm.tk";
#endif

    /// <summary>
    /// The client that will make all of the requests
    /// </summary>
    /// <param name="credentials">The users credentials</param>
    public DSMClient(string api_key)
    {
        this.api_key = api_key;
        client = GetClient();
        Users = new(this);
    }

    private HttpClient GetClient()
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("api_key", api_key);

        return client;
    }

    // GET DATA
    internal HttpResponseMessage Get(string path) => Make(HttpMethod.Get, path);
    internal HttpResponseMessage Get(string path, IEnumerable<KeyValuePair<string, string>> formData) => Make(HttpMethod.Get, path, formData: formData);

    // POST DATA
    internal HttpResponseMessage Post(string path) => Make(HttpMethod.Post, path);
    internal HttpResponseMessage Post(string path, string body) => Make(HttpMethod.Post, path, body);
    internal HttpResponseMessage Post(string path, IEnumerable<KeyValuePair<string, string>> formData) => Make(HttpMethod.Post, path, formData: formData);

    // DELETE DATA
    internal HttpResponseMessage Delete(string path) => Make(HttpMethod.Delete, path);
    internal HttpResponseMessage Delete(string path, string body) => Make(HttpMethod.Delete, path, body);
    internal HttpResponseMessage Delete(string path, IEnumerable<KeyValuePair<string, string>> formData) => Make(HttpMethod.Delete, path, formData: formData);

    // PATCH DATA
    internal HttpResponseMessage Patch(string path) => Make(HttpMethod.Patch, path);
    internal HttpResponseMessage Patch(string path, string body) => Make(HttpMethod.Patch, path, body);
    internal HttpResponseMessage Patch(string path, IEnumerable<KeyValuePair<string, string>> formData) => Make(HttpMethod.Patch, path, formData: formData);

    private HttpResponseMessage Make(HttpMethod method, string path, string? body = "", IEnumerable<KeyValuePair<string, string>>? formData = null)
    {
        using HttpRequestMessage request = new(method, $"{HOST}{path}");
        if (!string.IsNullOrWhiteSpace(body))
            request.Content = new StringContent(body, System.Text.Encoding.UTF8);
        else if (formData != null)
            request.Content = new FormUrlEncodedContent(formData);
        HttpResponseMessage response = client.Send(request);
        if(response.StatusCode > System.Net.HttpStatusCode.BadRequest)throw new HttpRequestException($"Server returned with status code {response.StatusCode:D} {response.StatusCode:G}");
        return response;
    }
}
