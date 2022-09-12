using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDSM.NET.Exceptions;
using OpenDSM.NET.Models;

namespace OpenDSM.NET.Requests;
public enum ImageType
{
    Profile,
    Banner
}
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
        using HttpResponseMessage response = Get(client, $"/users/{id}");
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
        using HttpResponseMessage response = Get(client, $"/search/users?query={query}&page={page}&count={items_per_page}");
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

    public static bool UploadImage(DSMClient client, ImageType type, FileStream stream)
    {
        MemoryStream ms = new();
        stream.CopyTo(ms);
        return UploadImage(client, type, ms);
    }
    public static bool UploadImage(DSMClient client, ImageType type, MemoryStream stream)
    {
        return UploadImage(client, type, Encoding.UTF8.GetString(stream.GetBuffer()));
    }
    public static bool UploadImage(DSMClient client, ImageType type, string base64)
    {
        using HttpResponseMessage response = Post(client, $"/images/user/{type}", base64);
        return response.IsSuccessStatusCode;
    }
}
