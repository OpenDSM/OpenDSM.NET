global using static OpenDSM.NET.DSMClient;
namespace OpenDSM.NET;
public record UserCredentials(string username, string token);
public class DSMClient
{
    /// <summery>The host url</summery>
#if DEBUG
    public static readonly string HOST = "http://127.0.0.1:8080";
#else
    public static readonly string HOST = "http://api.opendsm.tk";
#endif
    private UserCredentials credentials;
    internal readonly HttpClient client;


    public DSMClient(UserCredentials credentials)
    {
        this.credentials = credentials;
        client = GetClient();
    }

    private HttpClient GetClient()
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("auth_user", credentials.username);
        client.DefaultRequestHeaders.Add("auth_token", credentials.token);

        return client;
    }
}
