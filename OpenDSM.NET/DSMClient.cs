using Newtonsoft.Json.Linq;
using OpenDSM.NET.Exceptions;

namespace OpenDSM.NET;
public record UserCredentials(string username, string token);
/// <summary>
/// The client that will make all of the requests
/// </summary>
public class DSMClient
{
    private UserCredentials credentials;
    internal readonly HttpClient client;

    /// <summary>
    /// The client that will make all of the requests
    /// </summary>
    /// <param name="credentials">The users credentials</param>
    public DSMClient(UserCredentials credentials)
    {
        this.credentials = credentials;
        client = GetClient();
        if (!ValidateCredentials(credentials, out string error))
        {
            throw new InvalidCredentialsException(credentials, error);
        }
    }

    private HttpClient GetClient()
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("auth_user", credentials.username);
        client.DefaultRequestHeaders.Add("auth_token", credentials.token);

        return client;
    }

    private bool ValidateCredentials(UserCredentials credentials, out string error)
    {
        using var response = Post(this, "/user/validate/token", new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("username", credentials.username),
            new KeyValuePair<string, string>("token", credentials.token)
        });
        if (response.IsSuccessStatusCode)
        {
            JObject json = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            bool success = (bool)json["success"];
            error = (string)json["message"];
            return success;
        }
        error = $"Server responded with status code {response.StatusCode}";
        return false;
    }
}
