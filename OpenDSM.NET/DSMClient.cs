namespace OpenDSM.NET;
public record UserCredentials(string username, string token);
public class DSMClient
{
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
