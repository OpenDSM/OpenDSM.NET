using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDSM.NET.Exceptions;
using OpenDSM.NET.Models;

namespace OpenDSM.NET.Requests;

/// <summary>
/// For all request made to the "user" endpoint
/// </summary>
public static class UserRequests
{
    /// <summary>
    /// Gets a users information
    /// </summary>
    /// <param name="client">The DSMClient</param>
    /// <param name="id">The users id</param>
    /// <returns></returns>
    public static DSMUser GetUser(DSMClient client, int id)
    {
        HttpClient http = client.client;
        using HttpRequestMessage request = new(HttpMethod.Get, $"{HOST}/user/{id}");
        using HttpResponseMessage response = http.Send(request);
        if (response.IsSuccessStatusCode)
        {
            return new DSMUser(response.Content.ReadAsStringAsync().Result);
        }
        string message = (string)JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result)["message"];
        if (message.StartsWith("No user found"))
        {
            throw new UnresolvedQueryResultException();
        }
        throw new HttpRequestException($"Server Responded with Error Code {response.StatusCode}");
    }

    public static DSMUser[] GetUserFromQuery(DSMClient client, string query, int page, int items_per_page)
    {
        HttpClient http = client.client;
        using HttpRequestMessage request = new(HttpMethod.Get, $"{HOST}/search/users?query={query}&page={page}&count={items_per_page}");
        using HttpResponseMessage response = http.Send(request);
        if (response.IsSuccessStatusCode)
        {
            JArray array = JsonConvert.DeserializeObject<JArray>(response.Content.ReadAsStringAsync().Result);
            DSMUser[] users = new DSMUser[array.Count()];
            Parallel.For(0, users.Length, i =>
            {
                users[i] = new(JsonConvert.SerializeObject(array[0]));
            });
            return users;
        }

        throw new UnresolvedQueryResultException();
    }

}
